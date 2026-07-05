using System.Linq;
using UnityEngine;

namespace RePvP;

public sealed class HunterAbilityController
{
    private readonly RePvPConfig _config;
    private readonly RoleManager _roleManager;
    private readonly ILocalPlayerResolver _localPlayerResolver;

    private float _pulseCooldownRemaining;
    private float _pulseDurationRemaining;

    public HunterAbilityController(
        RePvPConfig config,
        RoleManager roleManager,
        ILocalPlayerResolver localPlayerResolver)
    {
        _config = config;
        _roleManager = roleManager;
        _localPlayerResolver = localPlayerResolver;
    }

    public void Tick(float deltaTime, GamePhase phase)
    {
        if (_pulseCooldownRemaining > 0f)
        {
            _pulseCooldownRemaining -= deltaTime;
        }

        if (_pulseDurationRemaining > 0f)
        {
            _pulseDurationRemaining -= deltaTime;
            LogVisibleHeisters();
        }

        if (phase != GamePhase.Heist && phase != GamePhase.Extraction)
        {
            return;
        }

        if (!IsLocalHunter())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryPulseScan();
        }
    }

    private bool IsLocalHunter()
    {
        return _roleManager.Hunter != null && _localPlayerResolver.IsLocalPlayer(_roleManager.Hunter);
    }

    private void TryPulseScan()
    {
        if (_roleManager.Hunter == null)
        {
            return;
        }

        if (_pulseCooldownRemaining > 0f)
        {
            Plugin.Log.LogInfo($"Pulse Scan on cooldown: {_pulseCooldownRemaining:0.0}s remaining.");
            return;
        }

        _pulseDurationRemaining = _config.PulseScanDuration.Value;
        _pulseCooldownRemaining = _config.PulseScanCooldown.Value;
        Plugin.Log.LogWarning("Hunter used Pulse Scan.");
        LogVisibleHeisters();
    }

    private void LogVisibleHeisters()
    {
        var hunter = _roleManager.Hunter;
        if (hunter == null)
        {
            return;
        }

        var range = _config.PulseScanRange.Value;
        var visible = _roleManager.Heisters
            .Where(h => h.GameObject != null)
            .Select(h => new
            {
                Player = h,
                Distance = Vector3.Distance(hunter.Position, h.Position)
            })
            .Where(x => x.Distance <= range)
            .OrderBy(x => x.Distance)
            .ToList();

        if (visible.Count == 0)
        {
            Plugin.Log.LogInfo("Pulse Scan: no Heisters detected.");
            return;
        }

        foreach (var entry in visible)
        {
            Plugin.Log.LogInfo($"Pulse Scan: {entry.Player.DisplayName} at {entry.Distance:0.0}m.");
        }
    }
}
