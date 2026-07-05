using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
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

    private Harmony? _harmony;
    private DebugOverlayRenderer _debugOverlay = null!;
    private DebugCommandController _debugCommandController = null!;
    private RoundEventBus _roundEventBus = null!;
    private MessageFeed _messageFeed = null!;
    private RoundMessagePresenter _roundMessagePresenter = null!;
    private RoundStateBroadcaster? _roundStateBroadcaster;

    private void Awake()
    {
        Log = Logger;
        ModConfig = new RePvPConfig(Config);
        _roundEventBus = new RoundEventBus();
        _messageFeed = new MessageFeed();
        _roundMessagePresenter = new RoundMessagePresenter(_roundEventBus, _messageFeed);

        if (ModConfig.EnableNetworkBroadcastPlaceholder.Value)
        {
            _roundStateBroadcaster = new RoundStateBroadcaster(_roundEventBus);
        }

        RoundManager = new RoundManager(
            ModConfig,
            new UnityPlayerProvider(),
            new UnityLocalPlayerResolver(),
            _roundEventBus);
        _debugOverlay = new DebugOverlayRenderer(ModConfig, _messageFeed);
        _debugCommandController = new DebugCommandController(ModConfig, RoundManager);

        if (ModConfig.LogStartupDiagnostics.Value)
        {
            DependencyDiagnostics.LogLoadedAssemblies();
            DependencyDiagnostics.LogPatchTargetCandidates();
        }

        _harmony = new Harmony(PluginGuid);
        PatchBootstrap.ApplyPatches(_harmony);

        Logger.LogInfo($"{PluginName} {PluginVersion} loaded.");
    }

    private void OnDestroy()
    {
        _roundStateBroadcaster?.Dispose();
        _roundMessagePresenter?.Dispose();
        _messageFeed?.Clear();
        _harmony?.UnpatchSelf();
        _harmony = null;
    }

    private void Update()
    {
        if (!ModConfig.Enabled.Value)
        {
            return;
        }

        RoundManager.Tick(Time.deltaTime);
        _debugCommandController.Tick();
    }

    private void OnGUI()
    {
        if (!ModConfig.Enabled.Value)
        {
            return;
        }

        _debugOverlay.OnGui(RoundManager.GetSnapshot());
    }
}
