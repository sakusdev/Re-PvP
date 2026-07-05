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
        LogTypeCandidate("Player discovery", "PlayerAvatar", "PlayerController", "PlayerManager", "SemiFunc", "NetworkPlayer");
        LogTypeCandidate("Valuable cash-in", "ValuableObject", "Valuable", "ValuableDirector", "ExtractionPoint", "ShopManager", "StatsManager");
        LogTypeCandidate("Extraction", "ExtractionPoint", "ExtractionZone", "LevelExit", "TruckScreen", "RunManager", "RoundDirector");
    }

    private static void LogAssembly(Assembly assembly)
    {
        var name = assembly.GetName();
        Plugin.Log.LogInfo($"Assembly loaded: {name.Name} {name.Version} | {assembly.Location}");
    }

    private static void LogTypeCandidate(string label, params string[] candidates)
    {
        var type = PatchReflection.FindTypeByName(candidates);
        if (type == null)
        {
            Plugin.Log.LogInfo($"{label} candidate type: none found.");
            return;
        }

        Plugin.Log.LogInfo($"{label} candidate type: {type.FullName} in {type.Assembly.GetName().Name}");
    }
}
