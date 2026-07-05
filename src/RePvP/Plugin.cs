using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace RePvP;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "dev.sakus.repvp";
    public const string PluginName = "Re-PvP";
    public const string PluginVersion = "0.1.0";

    internal static ManualLogSource Log { get; private set; } = null!;
    internal static RePvPConfig ModConfig { get; private set; } = null!;
    internal static RoundManager RoundManager { get; private set; } = null!;

    private DebugOverlayRenderer _debugOverlay = null!;

    private void Awake()
    {
        Log = Logger;
        ModConfig = new RePvPConfig(Config);
        RoundManager = new RoundManager(ModConfig, new UnityPlayerProvider());
        _debugOverlay = new DebugOverlayRenderer(ModConfig);

        Logger.LogInfo($"{PluginName} {PluginVersion} loaded.");
    }

    private void Update()
    {
        if (!ModConfig.Enabled.Value)
        {
            return;
        }

        RoundManager.Tick(Time.deltaTime);
        HandleDebugKeys();
    }

    private void OnGUI()
    {
        if (!ModConfig.Enabled.Value)
        {
            return;
        }

        _debugOverlay.OnGui(RoundManager.GetSnapshot());
    }

    private void HandleDebugKeys()
    {
        if (!ModConfig.DebugKeysEnabled.Value)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            RoundManager.ForceStartRound();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            RoundManager.AddCashForDebug(ModConfig.DebugCashAmount.Value);
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            RoundManager.TriggerExtractionForDebug();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            RoundManager.EndRoundForDebug(Team.Heisters);
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            RoundManager.EndRoundForDebug(Team.Hunter);
        }
    }
}
