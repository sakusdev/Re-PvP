using System;

namespace RePvP;

public static class RoundEvents
{
    public static event Action<GamePhase, GamePhase>? PhaseChanged;
    public static event Action<int, int>? CashChanged;
    public static event Action<PlayerRef, int, int>? HeisterExtracted;
    public static event Action<Team, string>? RoundEnded;

    internal static void RaisePhaseChanged(GamePhase previous, GamePhase current)
    {
        PhaseChanged?.Invoke(previous, current);
    }

    internal static void RaiseCashChanged(int currentCash, int requiredCash)
    {
        CashChanged?.Invoke(currentCash, requiredCash);
    }

    internal static void RaiseHeisterExtracted(PlayerRef player, int extractedCount, int requiredCount)
    {
        HeisterExtracted?.Invoke(player, extractedCount, requiredCount);
    }

    internal static void RaiseRoundEnded(Team winner, string reason)
    {
        RoundEnded?.Invoke(winner, reason);
    }
}
