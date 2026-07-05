using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RePvP;

public sealed class RoundManager
{
    private readonly RePvPConfig _config;
    private readonly IPlayerProvider _playerProvider;
    private readonly RoleManager _roleManager = new();
    private readonly HunterAbilityController _hunterAbilityController;
    private readonly HunterStatController _hunterStatController;
    private readonly RoundEventBus _eventBus;
    private readonly HashSet<string> _extractedHeisterIds = new();

    private float _phaseTimer;
    private float _roundTimer;

    public RoundManager(
        RePvPConfig config,
        IPlayerProvider playerProvider,
        ILocalPlayerResolver localPlayerResolver,
        RoundEventBus eventBus)
    {
        _config = config;
        _playerProvider = playerProvider;
        _eventBus = eventBus;
        _hunterAbilityController = new HunterAbilityController(config, _roleManager, localPlayerResolver);
        _hunterStatController = new HunterStatController(config, _roleManager);
    }

    public GamePhase Phase { get; private set; } = GamePhase.WaitingForPlayers;
    public int CurrentCash { get; private set; }
    public int RequiredCash { get; private set; }

    public RoundSnapshot GetSnapshot()
    {
        return new RoundSnapshot(
            Phase,
            CurrentCash,
            RequiredCash,
            _phaseTimer,
            _roundTimer,
            _roleManager.Heisters.Count,
            _extractedHeisterIds.Count,
            _roleManager.Hunter?.DisplayName);
    }

    public void Tick(float deltaTime)
    {
        _hunterAbilityController.Tick(deltaTime, Phase);

        switch (Phase)
        {
            case GamePhase.WaitingForPlayers:
                break;
            case GamePhase.Preparation:
                TickPreparation(deltaTime);
                break;
            case GamePhase.Heist:
                TickHeist(deltaTime);
                break;
            case GamePhase.Alarm:
                TickAlarm(deltaTime);
                break;
            case GamePhase.Extraction:
                TickExtraction(deltaTime);
                break;
            case GamePhase.RoundEnd:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ForceStartRound()
    {
        var players = _playerProvider.GetPlayers();
        if (players.Count < 2)
        {
            Plugin.Log.LogWarning($"Need at least 2 players to start Re-PvP. Found {players.Count}.");
            return;
        }

        _roleManager.AssignTeams(players);
        _hunterStatController.ApplyHunterStats();
        _extractedHeisterIds.Clear();
        CurrentCash = 0;
        RequiredCash = Math.Max(1, _config.RequiredCashOverride.Value);
        _roundTimer = _config.RoundTimeLimit.Value;
        _eventBus.RaiseCashChanged(CurrentCash, RequiredCash);

        ChangePhase(GamePhase.Preparation, _config.PreparationTime.Value);
        Plugin.Log.LogInfo($"Round started. Required cash: ${RequiredCash:N0}. Round timer: {_roundTimer:0}s.");
    }

    public void AddCash(int amount)
    {
        if (Phase != GamePhase.Heist && Phase != GamePhase.Preparation)
        {
            return;
        }

        CurrentCash = Math.Max(0, CurrentCash + amount);
        Plugin.Log.LogInfo($"Cash updated: ${CurrentCash:N0} / ${RequiredCash:N0}");
        _eventBus.RaiseCashChanged(CurrentCash, RequiredCash);

        if (CurrentCash >= RequiredCash && Phase == GamePhase.Heist)
        {
            TriggerAlarm();
        }
    }

    public void AddCashForDebug(int amount)
    {
        Plugin.Log.LogInfo($"Debug: adding ${amount:N0} cash.");
        AddCash(amount);
    }

    public void TriggerExtractionForDebug()
    {
        Plugin.Log.LogInfo("Debug: forcing extraction phase.");
        TriggerAlarm();
    }

    public bool ExtractNextHeisterForDebug()
    {
        if (Phase != GamePhase.Extraction)
        {
            Plugin.Log.LogInfo($"Debug extraction ignored during phase: {Phase}");
            return false;
        }

        var nextHeister = _roleManager.Heisters.FirstOrDefault(h => !_extractedHeisterIds.Contains(h.Id));
        if (nextHeister == null)
        {
            Plugin.Log.LogInfo("Debug extraction ignored: no remaining Heisters to extract.");
            return false;
        }

        Plugin.Log.LogInfo($"Debug: extracting next Heister: {nextHeister.DisplayName}");
        return MarkHeisterExtracted(nextHeister);
    }

    public void EndRoundForDebug(Team winner)
    {
        EndRound(winner, "Debug forced round end.");
    }

    public void ResetToWaitingForDebug()
    {
        Plugin.Log.LogWarning("Debug: resetting Re-PvP round state to WaitingForPlayers.");
        _hunterStatController.ResetHunterStats();
        _roleManager.Clear();
        _extractedHeisterIds.Clear();
        CurrentCash = 0;
        RequiredCash = 0;
        _phaseTimer = 0f;
        _roundTimer = 0f;
        _eventBus.RaiseCashChanged(CurrentCash, RequiredCash);
        ChangePhase(GamePhase.WaitingForPlayers);
    }

    public void LogDebugState()
    {
        var snapshot = GetSnapshot();
        Plugin.Log.LogInfo("=== Re-PvP Debug State ===");
        Plugin.Log.LogInfo($"Phase: {snapshot.Phase}");
        Plugin.Log.LogInfo($"Cash: ${snapshot.CurrentCash:N0} / ${snapshot.RequiredCash:N0} ({snapshot.CashProgress:P0})");
        Plugin.Log.LogInfo($"PhaseTimeRemaining: {snapshot.PhaseTimeRemaining:0.0}s");
        Plugin.Log.LogInfo($"RoundTimeRemaining: {snapshot.RoundTimeRemaining:0.0}s");
        Plugin.Log.LogInfo($"Hunter: {snapshot.HunterName ?? "None"}");
        Plugin.Log.LogInfo($"Heisters: {snapshot.HeisterCount}");
        Plugin.Log.LogInfo($"Extracted: {snapshot.ExtractedCount}");
        LogTeamDetails();
        Plugin.Log.LogInfo("==========================");
    }

    public void LogTeamDetails()
    {
        Plugin.Log.LogInfo("--- Re-PvP Teams ---");
        Plugin.Log.LogInfo($"Hunter: {_roleManager.Hunter?.DisplayName ?? "None"}");

        if (_roleManager.Heisters.Count == 0)
        {
            Plugin.Log.LogInfo("Heisters: none");
        }
        else
        {
            foreach (var heister in _roleManager.Heisters)
            {
                var extracted = _extractedHeisterIds.Contains(heister.Id) ? "extracted" : "active";
                Plugin.Log.LogInfo($"Heister: {heister.DisplayName} [{extracted}]");
            }
        }

        Plugin.Log.LogInfo("--------------------");
    }

    public bool TryMarkHeisterExtracted(GameObject gameObject)
    {
        var player = FindKnownPlayer(gameObject);
        return player != null && MarkHeisterExtracted(player);
    }

    public bool MarkHeisterExtracted(PlayerRef player)
    {
        if (Phase != GamePhase.Extraction)
        {
            return false;
        }

        if (_roleManager.GetTeam(player) != Team.Heisters)
        {
            return false;
        }

        if (!_extractedHeisterIds.Add(player.Id))
        {
            return false;
        }

        Plugin.Log.LogInfo($"Heister extracted: {player.DisplayName} ({_extractedHeisterIds.Count}/{_config.MinimumHeistersToExtract.Value})");
        _eventBus.RaiseHeisterExtracted(player, _extractedHeisterIds.Count, _config.MinimumHeistersToExtract.Value);

        if (_extractedHeisterIds.Count >= _config.MinimumHeistersToExtract.Value)
        {
            EndRound(Team.Heisters, "Required Heisters extracted.");
        }

        return true;
    }

    private PlayerRef? FindKnownPlayer(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return null;
        }

        var allPlayers = _roleManager.Heisters
            .Concat(_roleManager.Hunter != null ? new[] { _roleManager.Hunter } : Array.Empty<PlayerRef>());

        foreach (var player in allPlayers)
        {
            if (player.GameObject == gameObject || gameObject.transform.IsChildOf(player.GameObject.transform))
            {
                return player;
            }
        }

        return null;
    }

    private void TickPreparation(float deltaTime)
    {
        _phaseTimer -= deltaTime;
        _roundTimer -= deltaTime;

        if (_phaseTimer <= 0f)
        {
            ChangePhase(GamePhase.Heist);
        }
    }

    private void TickHeist(float deltaTime)
    {
        _roundTimer -= deltaTime;

        if (_roundTimer <= 0f)
        {
            EndRound(Team.Hunter, "Time expired before quota was reached.");
            return;
        }

        if (CurrentCash >= RequiredCash)
        {
            TriggerAlarm();
        }
    }

    private void TickAlarm(float deltaTime)
    {
        _phaseTimer -= deltaTime;
        if (_phaseTimer <= 0f)
        {
            ChangePhase(GamePhase.Extraction, _config.ExtractionTime.Value);
            Plugin.Log.LogWarning("EXTRACTION OPEN. Heisters must escape now.");
        }
    }

    private void TickExtraction(float deltaTime)
    {
        _phaseTimer -= deltaTime;
        _roundTimer -= deltaTime;

        if (_extractedHeisterIds.Count >= _config.MinimumHeistersToExtract.Value)
        {
            EndRound(Team.Heisters, "Required Heisters extracted.");
            return;
        }

        if (_phaseTimer <= 0f)
        {
            EndRound(Team.Hunter, "Extraction timer expired.");
        }
    }

    private void TriggerAlarm()
    {
        if (Phase != GamePhase.Heist && Phase != GamePhase.Preparation)
        {
            Plugin.Log.LogInfo($"Alarm trigger ignored during phase: {Phase}");
            return;
        }

        Plugin.Log.LogWarning("QUOTA REACHED. ALARM TRIGGERED. Hunter has been notified.");
        ChangePhase(GamePhase.Alarm, 3f);
    }

    private void EndRound(Team winner, string reason)
    {
        if (Phase == GamePhase.RoundEnd)
        {
            return;
        }

        ChangePhase(GamePhase.RoundEnd);
        var winnerName = winner == Team.Heisters ? "HEISTERS" : "HUNTER";
        Plugin.Log.LogWarning($"ROUND END: {winnerName} WIN. Reason: {reason}");
        Plugin.Log.LogInfo($"Final cash: ${CurrentCash:N0} / ${RequiredCash:N0}. Extracted: {_extractedHeisterIds.Count}.");
        _eventBus.RaiseRoundEnded(winner, reason);

        _hunterStatController.ResetHunterStats();
        _roleManager.Clear();
        _extractedHeisterIds.Clear();
    }

    private void ChangePhase(GamePhase phase, float phaseTimer = 0f)
    {
        var previous = Phase;
        Phase = phase;
        _phaseTimer = phaseTimer;
        Plugin.Log.LogInfo($"Phase changed: {Phase} ({_phaseTimer:0.0}s)");
        _eventBus.RaisePhaseChanged(previous, Phase);
    }
}
