using System;

namespace RePvP;

public sealed class RoundMessagePresenter : IDisposable
{
    private readonly RoundEventBus _eventBus;
    private readonly MessageFeed _messages;
    private bool _disposed;

    public RoundMessagePresenter(RoundEventBus eventBus, MessageFeed messages)
    {
        _eventBus = eventBus;
        _messages = messages;
        _eventBus.PhaseChanged += OnPhaseChanged;
        _eventBus.CashChanged += OnCashChanged;
        _eventBus.HeisterExtracted += OnHeisterExtracted;
        _eventBus.RoundEnded += OnRoundEnded;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _eventBus.PhaseChanged -= OnPhaseChanged;
        _eventBus.CashChanged -= OnCashChanged;
        _eventBus.HeisterExtracted -= OnHeisterExtracted;
        _eventBus.RoundEnded -= OnRoundEnded;
        _disposed = true;
    }

    private void OnPhaseChanged(GamePhase previous, GamePhase next)
    {
        switch (next)
        {
            case GamePhase.Preparation:
                _messages.Push("PREPARATION PHASE - Heisters get ready.");
                break;
            case GamePhase.Heist:
                _messages.Push("HEIST PHASE - Get the valuables.");
                break;
            case GamePhase.Alarm:
                _messages.Push("QUOTA REACHED - ALARM TRIGGERED.", 5f);
                break;
            case GamePhase.Extraction:
                _messages.Push("EXTRACTION OPEN - Get out alive.", 5f);
                break;
            case GamePhase.RoundEnd:
                _messages.Push("ROUND ENDED.", 5f);
                break;
        }
    }

    private void OnCashChanged(int currentCash, int requiredCash)
    {
        if (requiredCash <= 0)
        {
            return;
        }

        _messages.Push($"Cash: ${currentCash:N0} / ${requiredCash:N0}", 2.5f);
    }

    private void OnHeisterExtracted(PlayerRef player, int extractedCount, int requiredCount)
    {
        _messages.Push($"{player.DisplayName} extracted ({extractedCount}/{requiredCount}).", 4f);
    }

    private void OnRoundEnded(Team winner, string reason)
    {
        var winnerName = winner == Team.Heisters ? "HEISTERS WIN" : "HUNTER WINS";
        _messages.Push($"{winnerName} - {reason}", 6f);
    }
}
