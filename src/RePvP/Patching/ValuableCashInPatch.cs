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

        var postfixName = method.IsStatic ? nameof(PostfixStatic) : nameof(PostfixInstance);
        var postfix = typeof(ValuableCashInPatch).GetMethod(postfixName, BindingFlags.Static | BindingFlags.NonPublic);
        if (postfix == null)
        {
            Plugin.Log.LogWarning($"Valuable cash-in patch skipped: postfix method missing: {postfixName}.");
            return;
        }

        harmony.Patch(method, postfix: new HarmonyMethod(postfix));
        Plugin.Log.LogInfo($"Valuable cash-in patch applied: {type.FullName}.{method.Name}");
    }

    private static void PostfixInstance(object __instance, object[] __args, MethodBase __originalMethod)
    {
        var value = ResolveCashValue(__args, __originalMethod, __instance);
        NotifyIfValid(value, __instance, __originalMethod);
    }

    private static void PostfixStatic(object[] __args, MethodBase __originalMethod)
    {
        var value = ResolveCashValue(__args, __originalMethod, null);
        NotifyIfValid(value, __originalMethod, __originalMethod);
    }

    private static int ResolveCashValue(object[] __args, MethodBase __originalMethod, object? instance)
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

        if (value > 0)
        {
            return value;
        }

        return PatchReflection.TryReadIntMember(
            instance,
            "cashValue",
            "value",
            "price",
            "amount",
            "money",
            "totalValue");
    }

    private static void NotifyIfValid(int value, object source, MethodBase originalMethod)
    {
        if (value <= 0)
        {
            Plugin.Log.LogInfo($"Valuable cash-in patch fired but no positive value was found for {originalMethod.DeclaringType?.FullName}.{originalMethod.Name}.");
            return;
        }

        ValuableCashInBridge.OnValuableCashed(value, source);
    }
}
