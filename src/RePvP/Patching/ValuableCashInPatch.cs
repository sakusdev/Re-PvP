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
        harmony.Patch(method, postfix: new HarmonyMethod(postfix));
        Plugin.Log.LogInfo($"Valuable cash-in patch applied: {type.FullName}.{method.Name}");
    }

    private static void Postfix(object __instance)
    {
        var value = PatchReflection.TryReadIntMember(
            __instance,
            "cashValue",
            "value",
            "price",
            "amount",
            "money",
            "totalValue");

        if (value <= 0)
        {
            Plugin.Log.LogInfo($"Valuable cash-in patch fired but no value field was found on {__instance.GetType().FullName}.");
            return;
        }

        ValuableCashInBridge.OnValuableCashed(value, __instance);
    }
}
