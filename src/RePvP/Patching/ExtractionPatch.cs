using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace RePvP;

public static class ExtractionPatch
{
    public static void TryPatch(Harmony harmony)
    {
        var typeCandidates = ConfigParsing.SplitCsv(Plugin.ModConfig.ExtractionTypeCandidates.Value);
        var methodCandidates = ConfigParsing.SplitCsv(Plugin.ModConfig.ExtractionMethodCandidates.Value);

        if (typeCandidates.Length == 0 || methodCandidates.Length == 0)
        {
            Plugin.Log.LogInfo("Extraction patch skipped: no type or method candidates configured.");
            return;
        }

        var type = PatchReflection.FindTypeByName(typeCandidates);
        if (type == null)
        {
            Plugin.Log.LogInfo("Extraction patch skipped: target type not found yet.");
            return;
        }

        var method = PatchReflection.FindMethod(type, methodCandidates);
        if (method == null)
        {
            Plugin.Log.LogInfo($"Extraction patch skipped: no candidate method found on {type.FullName}.");
            return;
        }

        var postfix = typeof(ExtractionPatch).GetMethod(nameof(Postfix), BindingFlags.Static | BindingFlags.NonPublic);
        if (postfix == null)
        {
            Plugin.Log.LogWarning("Extraction patch skipped: postfix method missing.");
            return;
        }

        harmony.Patch(method, postfix: new HarmonyMethod(postfix));
        Plugin.Log.LogInfo($"Extraction patch applied: {type.FullName}.{method.Name}");
    }

    private static void Postfix(object[] __args, MethodBase __originalMethod)
    {
        foreach (var arg in __args)
        {
            if (TryNotifyFromArgument(arg))
            {
                return;
            }
        }

        Plugin.Log.LogInfo($"Extraction patch fired on {__originalMethod.DeclaringType?.FullName}.{__originalMethod.Name}, but no player object argument was detected.");
    }

    private static bool TryNotifyFromArgument(object? arg)
    {
        switch (arg)
        {
            case Collider collider:
                return RePvPApi.NotifyPlayerEnteredExtraction(collider.gameObject);
            case GameObject gameObject:
                return RePvPApi.NotifyPlayerEnteredExtraction(gameObject);
            case Component component:
                return RePvPApi.NotifyPlayerEnteredExtraction(component.gameObject);
            default:
                return false;
        }
    }
}
