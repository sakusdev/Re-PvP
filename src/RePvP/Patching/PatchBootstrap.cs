using HarmonyLib;

namespace RePvP;

public static class PatchBootstrap
{
    public static void ApplyPatches(Harmony harmony)
    {
        if (!Plugin.ModConfig.EnableHarmonyPatches.Value)
        {
            Plugin.Log.LogInfo("Harmony patches are disabled by config.");
            return;
        }

        TryApply("valuable cash-in", () => ValuableCashInPatch.TryPatch(harmony));
        TryApply("extraction", () => ExtractionPatch.TryPatch(harmony));
        TryApply("player discovery", () => PlayerDiscoveryPatch.TryPatch(harmony));
    }

    private static void TryApply(string name, System.Action patchAction)
    {
        try
        {
            patchAction();
        }
        catch (System.Exception ex)
        {
            Plugin.Log.LogWarning($"Failed to apply {name} patch: {ex}");
        }
    }
}
