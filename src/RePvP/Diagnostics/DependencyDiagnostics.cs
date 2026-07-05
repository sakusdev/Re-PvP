using System;
using System.Linq;
using System.Reflection;

namespace RePvP;

public static class DependencyDiagnostics
{
    private static readonly string[] ImportantAssemblies =
    {
        "BepInEx",
        "0Harmony",
        "UnityEngine",
        "UnityEngine.CoreModule",
        "Assembly-CSharp"
    };

    public static void LogLoadedAssemblies()
    {
        Plugin.Log.LogInfo("Dependency diagnostics start.");

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var expectedName in ImportantAssemblies)
        {
            var match = assemblies.FirstOrDefault(a => string.Equals(a.GetName().Name, expectedName, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                Plugin.Log.LogWarning($"Assembly not currently loaded: {expectedName}");
                continue;
            }

            LogAssembly(match);
        }

        Plugin.Log.LogInfo("Dependency diagnostics end.");
    }

    public static void LogPatchTargetCandidates()
    {
        LogTypeCandidate("Player discovery", ConfigParsing.SplitCsv(Plugin.ModConfig.PlayerTypeCandidates.Value));
        LogTypeCandidate("Valuable cash-in", ConfigParsing.SplitCsv(Plugin.ModConfig.CashInTypeCandidates.Value));
        LogTypeCandidate("Extraction", ConfigParsing.SplitCsv(Plugin.ModConfig.ExtractionTypeCandidates.Value));
    }

    private static void LogAssembly(Assembly assembly)
    {
        var name = assembly.GetName();
        Plugin.Log.LogInfo($"Assembly loaded: {name.Name} {name.Version} | {assembly.Location}");
    }

    private static void LogTypeCandidate(string label, params string[] candidates)
    {
        if (candidates.Length == 0)
        {
            Plugin.Log.LogInfo($"{label} candidate type: no candidates configured.");
            return;
        }

        var type = PatchReflection.FindTypeByName(candidates);
        if (type == null)
        {
            Plugin.Log.LogInfo($"{label} candidate type: none found.");
            return;
        }

        Plugin.Log.LogInfo($"{label} candidate type: {type.FullName} in {type.Assembly.GetName().Name}");
    }
}
