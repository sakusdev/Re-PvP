using System.Collections.Generic;
using UnityEngine;

namespace RePvP;

public static class DebugPlayerRegistry
{
    private static readonly List<GameObject> DebugPlayers = new();

    public static IReadOnlyList<GameObject> Players => DebugPlayers;

    public static bool HasDebugPlayers => DebugPlayers.Count > 0;

    public static void Register(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return;
        }

        if (DebugPlayers.Contains(gameObject))
        {
            return;
        }

        DebugPlayers.Add(gameObject);
        Plugin.Log.LogInfo($"Debug player registered: {gameObject.name} ({gameObject.GetInstanceID()})");
    }

    public static void Clear()
    {
        foreach (var player in DebugPlayers)
        {
            if (player != null && player.name.StartsWith("RePvP_DebugPlayer"))
            {
                Object.Destroy(player);
            }
        }

        DebugPlayers.Clear();
        Plugin.Log.LogInfo("Debug player registry cleared.");
    }
}
