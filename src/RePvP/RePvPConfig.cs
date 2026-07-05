using BepInEx.Configuration;

namespace RePvP;

public sealed class RePvPConfig
{
    public ConfigEntry<bool> Enabled { get; }
    public ConfigEntry<bool> EnableHarmonyPatches { get; }
    public ConfigEntry<bool> DebugKeysEnabled { get; }
    public ConfigEntry<bool> DebugOverlayEnabled { get; }
    public ConfigEntry<float> DebugOverlayX { get; }
    public ConfigEntry<float> DebugOverlayY { get; }

    public ConfigEntry<float> PreparationTime { get; }
    public ConfigEntry<float> RoundTimeLimit { get; }
    public ConfigEntry<float> ExtractionTime { get; }
    public ConfigEntry<float> RequiredCashPercent { get; }
    public ConfigEntry<int> RequiredCashOverride { get; }

    public ConfigEntry<float> HunterSpeedMultiplier { get; }
    public ConfigEntry<float> PulseScanDuration { get; }
    public ConfigEntry<float> PulseScanCooldown { get; }
    public ConfigEntry<float> PulseScanRange { get; }

    public ConfigEntry<int> MinimumHeistersToExtract { get; }
    public ConfigEntry<int> DebugCashAmount { get; }

    public RePvPConfig(ConfigFile config)
    {
        Enabled = config.Bind("General", "Enabled", true, "Enable Re-PvP mode.");
        EnableHarmonyPatches = config.Bind("General", "EnableHarmonyPatches", true, "Enable tentative Harmony patches for R.E.P.O. integration.");
        DebugKeysEnabled = config.Bind("Debug", "DebugKeysEnabled", true, "Enable F6-F10 debug controls.");
        DebugOverlayEnabled = config.Bind("Debug", "DebugOverlayEnabled", true, "Show the simple IMGUI debug overlay.");
        DebugOverlayX = config.Bind("Debug", "DebugOverlayX", 20f, "Debug overlay X position.");
        DebugOverlayY = config.Bind("Debug", "DebugOverlayY", 20f, "Debug overlay Y position.");

        PreparationTime = config.Bind("Round", "PreparationTime", 30f, "Seconds before the Hunter becomes active.");
        RoundTimeLimit = config.Bind("Round", "RoundTimeLimit", 720f, "Total round time in seconds.");
        ExtractionTime = config.Bind("Round", "ExtractionTime", 90f, "Extraction phase duration in seconds.");
        RequiredCashPercent = config.Bind("Round", "RequiredCashPercent", 0.60f, "Required cash as a ratio of estimated map value.");
        RequiredCashOverride = config.Bind("Round", "RequiredCashOverride", 45000, "Temporary fixed cash requirement until real map value hooks exist.");

        HunterSpeedMultiplier = config.Bind("Hunter", "HunterSpeedMultiplier", 1.15f, "Hunter movement speed multiplier. Hook not yet implemented.");
        PulseScanDuration = config.Bind("Hunter", "PulseScanDuration", 4f, "Hunter Pulse Scan reveal duration.");
        PulseScanCooldown = config.Bind("Hunter", "PulseScanCooldown", 45f, "Hunter Pulse Scan cooldown.");
        PulseScanRange = config.Bind("Hunter", "PulseScanRange", 40f, "Hunter Pulse Scan range in Unity units.");

        MinimumHeistersToExtract = config.Bind("Extraction", "MinimumHeistersToExtract", 1, "Minimum extracted Heisters required for Heister victory.");
        DebugCashAmount = config.Bind("Debug", "DebugCashAmount", 5000, "Cash added by F7 for testing.");
    }
}
