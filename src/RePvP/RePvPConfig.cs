using BepInEx.Configuration;

namespace RePvP;

public sealed class RePvPConfig
{
    public ConfigEntry<bool> Enabled { get; }
    public ConfigEntry<bool> EnableHarmonyPatches { get; }
    public ConfigEntry<bool> LogStartupDiagnostics { get; }
    public ConfigEntry<bool> EnableNetworkBroadcastPlaceholder { get; }
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

    public ConfigEntry<string> PlayerTypeCandidates { get; }
    public ConfigEntry<string> PlayerMethodCandidates { get; }
    public ConfigEntry<string> CashInTypeCandidates { get; }
    public ConfigEntry<string> CashInMethodCandidates { get; }
    public ConfigEntry<string> ExtractionTypeCandidates { get; }
    public ConfigEntry<string> ExtractionMethodCandidates { get; }

    public ConfigEntry<int> MinimumHeistersToExtract { get; }
    public ConfigEntry<int> DebugCashAmount { get; }

    public RePvPConfig(ConfigFile config)
    {
        Enabled = config.Bind("General", "Enabled", true, "Enable Re-PvP mode.");
        EnableHarmonyPatches = config.Bind("General", "EnableHarmonyPatches", false, "Enable tentative Harmony patches for R.E.P.O. integration. Keep this false until exact target methods are verified.");
        LogStartupDiagnostics = config.Bind("General", "LogStartupDiagnostics", true, "Log loaded assemblies and tentative hook candidate types at startup.");
        EnableNetworkBroadcastPlaceholder = config.Bind("General", "EnableNetworkBroadcastPlaceholder", false, "Enable log-only round state broadcast placeholder for future network sync work.");
        DebugKeysEnabled = config.Bind("Debug", "DebugKeysEnabled", true, "Enable F4-F12 debug controls.");
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

        PlayerTypeCandidates = config.Bind("Hooks", "PlayerTypeCandidates", "PlayerAvatar,PlayerController,PlayerManager,SemiFunc,NetworkPlayer", "Comma-separated player/session type candidates for tentative Harmony discovery.");
        PlayerMethodCandidates = config.Bind("Hooks", "PlayerMethodCandidates", "Start,Awake,OnEnable,Spawn,SpawnPlayer", "Comma-separated player discovery method candidates.");
        CashInTypeCandidates = config.Bind("Hooks", "CashInTypeCandidates", "ValuableObject,Valuable,ValuableDirector,ExtractionPoint,ShopManager,StatsManager", "Comma-separated cash-in type candidates.");
        CashInMethodCandidates = config.Bind("Hooks", "CashInMethodCandidates", "CashIn,OnCashIn,Sell,AddMoney,AddCash,ValuableCashed", "Comma-separated cash-in method candidates.");
        ExtractionTypeCandidates = config.Bind("Hooks", "ExtractionTypeCandidates", "ExtractionPoint,ExtractionZone,LevelExit,TruckScreen,RunManager,RoundDirector", "Comma-separated extraction type candidates.");
        ExtractionMethodCandidates = config.Bind("Hooks", "ExtractionMethodCandidates", "ExtractPlayer,OnPlayerEnter,OnTriggerEnter,PlayerExtracted,LeaveLevel", "Comma-separated extraction method candidates.");

        MinimumHeistersToExtract = config.Bind("Extraction", "MinimumHeistersToExtract", 1, "Minimum extracted Heisters required for Heister victory.");
        DebugCashAmount = config.Bind("Debug", "DebugCashAmount", 5000, "Cash added by F7 for testing.");
    }
}
