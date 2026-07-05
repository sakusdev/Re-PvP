namespace RePvP;

public sealed class RoundStateBroadcaster
{
    private readonly RoundEventBus _eventBus;

    public RoundStateBroadcaster(RoundEventBus eventBus)
    {
        _eventBus = eventBus;
        _eventBus.PhaseChanged += OnRoundStateChanged;
        _eventBus.CashChanged += OnCashChanged;
        _eventBus.HeisterExtracted += OnHeisterExtracted;
        _eventBus.RoundEnded += OnRoundEnded;
    }

    public void Dispose()
    {
        _eventBus.PhaseChanged -= OnRoundStateChanged;
        _eventBus.CashChanged -= OnCashChanged;
        _eventBus.HeisterExtracted -= OnHeisterExtracted;
        _eventBus.RoundEnded -= OnRoundEnded;
    }

    public void BroadcastSnapshot(RoundSnapshot snapshot)
    {
        // TODO: Replace this log-only placeholder with R.E.P.O.'s actual networking/RPC layer.
        var json = RoundStateSerializer.ToJson(snapshot);
        Plugin.Log.LogInfo($"Round state broadcast placeholder: {json}");
    }

    private void OnRoundStateChanged(GamePhase previous, GamePhase current)
    {
        Plugin.Log.LogInfo($"Round state broadcast placeholder: phase {previous} -> {current}");
    }

    private void OnCashChanged(int currentCash, int requiredCash)
    {
        Plugin.Log.LogInfo($"Round state broadcast placeholder: cash ${currentCash:N0} / ${requiredCash:N0}");
    }

    private void OnHeisterExtracted(PlayerRef player, int extractedCount, int requiredCount)
    {
        Plugin.Log.LogInfo($"Round state broadcast placeholder: extracted {player.DisplayName} ({extractedCount}/{requiredCount})");
    }

    private void OnRoundEnded(Team winner, string reason)
    {
        Plugin.Log.LogInfo($"Round state broadcast placeholder: round ended {winner}, reason: {reason}");
    }
}
