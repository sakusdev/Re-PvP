using HarmonyLib;
using System.Reflection;

namespace RePvP;

public static class ValuableCashInPatch
{
    public static void TryPatch(Harmony harmony)
    {
        // TODO: Replace candidates with exact R.E.P.O. class/method names after decompilation.
        var type = PatchReflection.FindTypeByName(
            "ValuableObject",
            "Valuable",
            "ValuableDirector",
            "ExtractionPoint",
            "ShopManager",
            "StatsManager");

        if (type == null)
        {
            Plugin.Log.LogInfo("Valuable cash-in patch skipped: target type not found yet.");
            return;
        }

        var method = PatchReflection.FindMethod(
            type,
            "CashIn",
            "OnCashIn",
            "Sell",
            "AddMoney",
            "AddCash",
            "ValuableCashed");

        if (method == null)
        {
            Plugin.Log.LogInfo($"Valuable cash-in patch skipped: no candidate method found on {type.FullName}.");
            return;
        }

        var postfix = typeof(ValuableCashInPatch).GetMethod(nameof(Postfix), BindingFlags.Static | BindingFlags.NonPublic);
        if (postfix == null)
        {
            Plugin.Log.LogWarning("Valuable cash-in patch skipped: postfix method missing.");
            return;
        }

        harmony.Patch(method, postfix: new HarmonyMethod(postfix));
        Plugin.Log.LogInfo($"Valuable cash-in patch applied: {type.FullName}.{method.Name}");
    }

    private static void Postfix(object? __instance, object[] __args, MethodBase __originalMethod)
    {
        var value = PatchReflection.TryReadIntArgument(
            __args,
            __originalMethod,
            "cashValue",
            "value",
            "price",
            "amount",
            "money",
            "totalValue");

        if (value <= 0)
        {
            value = PatchReflection.TryReadIntMember(
                __instance,
                "cashValue",
                "value",
                "price",
                "amount",
                "money",
                "totalValue");
        }

        if (value <= 0)
        {
            Plugin.Log.LogInfo($"Valuable cash-in patch fired but no positive value was found for {__originalMethod.DeclaringType?.FullName}.{__originalMethod.Name}.");
            return;
        }

        ValuableCashInBridge.OnValuableCashed(value, __instance ?? __originalMethod);
    }
}
