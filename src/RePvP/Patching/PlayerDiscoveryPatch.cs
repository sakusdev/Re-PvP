using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RePvP;

public static class PlayerDiscoveryPatch
{
    private static readonly List<GameObject> SeenPlayerObjects = new();

    public static IReadOnlyList<GameObject> SeenPlayers => SeenPlayerObjects;

    public static void TryPatch(Harmony harmony)
    {
        // TODO: Replace candidates with exact R.E.P.O. player/session class names.
        var type = PatchReflection.FindTypeByName(
            "PlayerAvatar",
            "PlayerController",
            "PlayerManager",
            "SemiFunc",
            "NetworkPlayer");

        if (type == null)
        {
            Plugin.Log.LogInfo("Player discovery patch skipped: target type not found yet.");
            return;
        }

        var method = PatchReflection.FindMethod(
            type,
            "Start",
            "Awake",
            "OnEnable",
            "Spawn",
            "SpawnPlayer");

        if (method == null)
        {
            Plugin.Log.LogInfo($"Player discovery patch skipped: no candidate method found on {type.FullName}.");
            return;
        }

        if (method.IsStatic)
        {
            Plugin.Log.LogInfo($"Player discovery patch skipped: {type.FullName}.{method.Name} is static and cannot provide a player instance safely.");
            return;
        }

        var postfix = typeof(PlayerDiscoveryPatch).GetMethod(nameof(Postfix), BindingFlags.Static | BindingFlags.NonPublic);
        if (postfix == null)
        {
            Plugin.Log.LogWarning("Player discovery patch skipped: postfix method missing.");
            return;
        }

        harmony.Patch(method, postfix: new HarmonyMethod(postfix));
        Plugin.Log.LogInfo($"Player discovery patch applied: {type.FullName}.{method.Name}");
    }

    public static bool TryRegister(GameObject? gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        if (SeenPlayerObjects.Contains(gameObject))
        {
            return false;
        }

        SeenPlayerObjects.Add(gameObject);
        Plugin.Log.LogInfo($"Player object discovered: {gameObject.name} ({gameObject.GetInstanceID()})");
        return true;
    }

    private static void Postfix(object __instance)
    {
        if (__instance is Component component)
        {
            TryRegister(component.gameObject);
        }
    }
}
