using System;

namespace RePvP;

public sealed class RoundEventBus
{
    public event Action<GamePhase, GamePhase>? PhaseChanged;
    public event Action<int, int>? CashChanged;
    public event Action<PlayerRef, int, int>? HeisterExtracted;
    public event Action<Team, string>? RoundEnded;

    public void RaisePhaseChanged(GamePhase previous, GamePhase next)
    {
        PhaseChanged?.Invoke(previous, next);
    }

    public void RaiseCashChanged(int currentCash, int requiredCash)
    {
        CashChanged?.Invoke(currentCash, requiredCash);
    }

    public void RaiseHeisterExtracted(PlayerRef player, int extractedCount, int requiredCount)
    {
        HeisterExtracted?.Invoke(player, extractedCount, requiredCount);
    }

    public void RaiseRoundEnded(Team winner, string reason)
    {
        RoundEnded?.Invoke(winner, reason);
    }
}
