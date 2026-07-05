using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace RePvP;

public static class ExtractionPatch
{
    public static void TryPatch(Harmony harmony)
    {
        // TODO: Replace candidates with exact R.E.P.O. extraction/exit class and method names.
        var type = PatchReflection.FindTypeByName(
            "ExtractionPoint",
            "ExtractionZone",
            "LevelExit",
            "TruckScreen",
            "RunManager",
            "RoundDirector");

        if (type == null)
        {
            Plugin.Log.LogInfo("Extraction patch skipped: target type not found yet.");
            return;
        }

        var method = PatchReflection.FindMethod(
            type,
            "ExtractPlayer",
            "OnPlayerEnter",
            "OnTriggerEnter",
            "PlayerExtracted",
            "LeaveLevel");

        if (method == null)
        {
            Plugin.Log.LogInfo($"Extraction patch skipped: no candidate method found on {type.FullName}.");
            return;
        }

        var postfix = typeof(ExtractionPatch).GetMethod(nameof(Postfix), BindingFlags.Static | BindingFlags.NonPublic);
        harmony.Patch(method, postfix: new HarmonyMethod(postfix));
        Plugin.Log.LogInfo($"Extraction patch applied: {type.FullName}.{method.Name}");
    }

    private static void Postfix(object __instance, object? __0 = null)
    {
        // Harmony passes the first original argument as __0 when available.
        // This supports methods like OnTriggerEnter(Collider other) or ExtractPlayer(GameObject player).
        if (__0 is Collider collider)
        {
            RePvPApi.NotifyPlayerEnteredExtraction(collider.gameObject);
            return;
        }

        if (__0 is GameObject gameObject)
        {
            RePvPApi.NotifyPlayerEnteredExtraction(gameObject);
            return;
        }

        if (__0 is Component component)
        {
            RePvPApi.NotifyPlayerEnteredExtraction(component.gameObject);
            return;
        }

        Plugin.Log.LogInfo($"Extraction patch fired on {__instance.GetType().FullName}, but no player object argument was detected.");
    }
}
