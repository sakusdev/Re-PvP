using System;
using System.Linq;
using UnityEngine;

namespace RePvP;

public sealed class UnityLocalPlayerResolver : ILocalPlayerResolver
{
    private static readonly string[] LocalNameHints =
    {
        "Local",
        "Player",
        "Camera"
    };

    public GameObject? GetLocalPlayerObject()
    {
        // TODO: Replace with R.E.P.O.'s actual local player accessor.
        // Heuristic: first active object with a Camera in its children and player-like name.
        return UnityEngine.Object
            .FindObjectsOfType<GameObject>()
            .Where(go => go != null && go.activeInHierarchy)
            .Where(go => LocalNameHints.Any(hint => go.name.Contains(hint, StringComparison.OrdinalIgnoreCase)))
            .FirstOrDefault(go => go.GetComponentInChildren<Camera>() != null);
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
}
