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
    private readonly HashSet<string> _extractedHeisterIds = new();

    private float _phaseTimer;
    private float _roundTimer;

    public RoundManager(
        RePvPConfig config,
        IPlayerProvider playerProvider,
        ILocalPlayerResolver localPlayerResolver)
    {
        _config = config;
        _playerProvider = playerProvider;
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

    public void EndRoundForDebug(Team winner)
    {
        EndRound(winner, "Debug forced round end.");
    }

    public bool TryMarkHeisterExtracted(GameObject gameObject)
    {
        var player = FindKnownPlayer(gameObject);
        if (player == null)
        {
            return false;
        }

        MarkHeisterExtracted(player);
        return true;
    }

    public void MarkHeisterExtracted(PlayerRef player)
    {
        if (Phase != GamePhase.Extraction)
        {
            return;
        }

        if (_roleManager.GetTeam(player) != Team.Heisters)
        {
            return;
        }

        if (_extractedHeisterIds.Add(player.Id))
        {
            Plugin.Log.LogInfo($"Heister extracted: {player.DisplayName} ({_extractedHeisterIds.Count}/{_config.MinimumHeistersToExtract.Value})");
        }

        if (_extractedHeisterIds.Count >= _config.MinimumHeistersToExtract.Value)
        {
            EndRound(Team.Heisters, "Required Heisters extracted.");
        }
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
        if (Phase == GamePhase.Alarm || Phase == GamePhase.Extraction || Phase == GamePhase.RoundEnd)
        {
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

        _hunterStatController.ResetHunterStats();
        _roleManager.Clear();
        _extractedHeisterIds.Clear();
    }

    private void ChangePhase(GamePhase phase, float phaseTimer = 0f)
    {
        Phase = phase;
        _phaseTimer = phaseTimer;
        Plugin.Log.LogInfo($"Phase changed: {Phase} ({_phaseTimer:0.0}s)");
    }
}
