using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RePvP;

public sealed class UnityPlayerProvider : IPlayerProvider
{
    private static readonly string[] PlayerNameHints =
    {
        "Player",
        "SemiFunc",
        "Avatar",
        "Character"
    };

    public IReadOnlyList<PlayerRef> GetPlayers()
    {
        // TODO: Replace this heuristic with R.E.P.O.'s actual player/session API.
        // This intentionally avoids hard dependencies on unknown game classes so the skeleton stays portable.
        var candidates = UnityEngine.Object
            .FindObjectsOfType<GameObject>()
            .Where(IsLikelyPlayerObject)
            .Distinct()
            .Take(16)
            .Select((go, index) => new PlayerRef(
                id: go.GetInstanceID().ToString(),
                displayName: string.IsNullOrWhiteSpace(go.name) ? $"Player {index + 1}" : go.name,
                gameObject: go))
            .ToList();

        return candidates;
    }

    private static bool IsLikelyPlayerObject(GameObject gameObject)
    {
        if (gameObject == null || !gameObject.activeInHierarchy)
        {
            return false;
        }

        var name = gameObject.name ?? string.Empty;
        if (!PlayerNameHints.Any(hint => name.Contains(hint, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        // Prefer objects that look controllable/physical.
        return gameObject.GetComponent<CharacterController>() != null
            || gameObject.GetComponent<Rigidbody>() != null
            || gameObject.GetComponentInChildren<Camera>() != null;
    }
}
