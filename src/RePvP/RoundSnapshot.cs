namespace RePvP;

public sealed class RoundSnapshot
{
    public RoundSnapshot(
        GamePhase phase,
        int currentCash,
        int requiredCash,
        float phaseTimeRemaining,
        float roundTimeRemaining,
        int heisterCount,
        int extractedCount,
        string? hunterName)
    {
        Phase = phase;
        CurrentCash = currentCash;
        RequiredCash = requiredCash;
        PhaseTimeRemaining = phaseTimeRemaining;
        RoundTimeRemaining = roundTimeRemaining;
        HeisterCount = heisterCount;
        ExtractedCount = extractedCount;
        HunterName = hunterName;
    }

    public GamePhase Phase { get; }
    public int CurrentCash { get; }
    public int RequiredCash { get; }
    public float PhaseTimeRemaining { get; }
    public float RoundTimeRemaining { get; }
    public int HeisterCount { get; }
    public int ExtractedCount { get; }
    public string? HunterName { get; }

    public float CashProgress => RequiredCash <= 0 ? 0f : (float)CurrentCash / RequiredCash;
}
