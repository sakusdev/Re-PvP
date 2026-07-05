namespace RePvP;

public sealed class RoundStatePacket
{
    public string Phase { get; set; } = GamePhase.WaitingForPlayers.ToString();
    public int CurrentCash { get; set; }
    public int RequiredCash { get; set; }
    public float PhaseTimeRemaining { get; set; }
    public float RoundTimeRemaining { get; set; }
    public int HeisterCount { get; set; }
    public int ExtractedCount { get; set; }
    public string HunterName { get; set; } = string.Empty;
    public float CashProgress { get; set; }

    public static RoundStatePacket FromSnapshot(RoundSnapshot snapshot)
    {
        return new RoundStatePacket
        {
            Phase = snapshot.Phase.ToString(),
            CurrentCash = snapshot.CurrentCash,
            RequiredCash = snapshot.RequiredCash,
            PhaseTimeRemaining = snapshot.PhaseTimeRemaining,
            RoundTimeRemaining = snapshot.RoundTimeRemaining,
            HeisterCount = snapshot.HeisterCount,
            ExtractedCount = snapshot.ExtractedCount,
            HunterName = snapshot.HunterName ?? string.Empty,
            CashProgress = snapshot.CashProgress
        };
    }
}
