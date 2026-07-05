using System;
using System.Linq;
using UnityEngine;

namespace RePvP;

public sealed class UnityLocalPlayerResolver : ILocalPlayerResolver
{
    private const float RetryIntervalSeconds = 2f;

    private static readonly string[] LocalNameHints =
    {
        "Local",
        "Player",
        "Camera"
    };

    private GameObject? _cachedLocalPlayer;
    private float _nextRetryTime;

    public GameObject? GetLocalPlayerObject()
    {
        // TODO: Replace with R.E.P.O.'s actual local player accessor.
        // This cache avoids running a scene-wide search every frame while Pulse Scan input is checked.
        if (_cachedLocalPlayer != null && _cachedLocalPlayer.activeInHierarchy)
        {
            return _cachedLocalPlayer;
        }

        if (Time.unscaledTime < _nextRetryTime)
        {
            return _cachedLocalPlayer;
        }

        _nextRetryTime = Time.unscaledTime + RetryIntervalSeconds;
        _cachedLocalPlayer = FindLocalPlayerObject();

        if (_cachedLocalPlayer != null)
        {
            Plugin.Log.LogInfo($"Local player candidate cached: {_cachedLocalPlayer.name} ({_cachedLocalPlayer.GetInstanceID()})");
        }

        return _cachedLocalPlayer;
    }

    public bool IsLocalPlayer(PlayerRef player)
    {
        var local = GetLocalPlayerObject();
        if (local == null || player.GameObject == null)
        {
            return false;
        }

        return local == player.GameObject
            || local.transform.IsChildOf(player.GameObject.transform)
            || player.GameObject.transform.IsChildOf(local.transform);
    }

    private static GameObject? FindLocalPlayerObject()
    {
        return UnityEngine.Object
            .FindObjectsOfType<GameObject>()
            .Where(go => go != null && go.activeInHierarchy)
            .Where(go => LocalNameHints.Any(hint => go.name.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0))
            .FirstOrDefault(go => go.GetComponentInChildren<Camera>() != null);
    }
}
