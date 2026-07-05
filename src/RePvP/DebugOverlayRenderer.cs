using UnityEngine;

namespace RePvP;

public sealed class DebugOverlayRenderer
{
    private readonly RePvPConfig _config;

    public DebugOverlayRenderer(RePvPConfig config)
    {
        _config = config;
    }

    public void OnGui(RoundSnapshot snapshot)
    {
        if (!_config.DebugOverlayEnabled.Value)
        {
            return;
        }

        var x = _config.DebugOverlayX.Value;
        var y = _config.DebugOverlayY.Value;
        const int width = 460;
        const int height = 210;

        GUI.Box(new Rect(x, y, width, height), "Re-PvP: Heist & Hunter");

        var line = y + 28;
        DrawLine(x, ref line, $"Phase: {snapshot.Phase}");
        DrawLine(x, ref line, $"Cash: ${snapshot.CurrentCash:N0} / ${snapshot.RequiredCash:N0} ({snapshot.CashProgress:P0})");
        DrawLine(x, ref line, $"Round Time: {FormatTime(snapshot.RoundTimeRemaining)}");
        DrawLine(x, ref line, $"Phase Time: {FormatTime(snapshot.PhaseTimeRemaining)}");
        DrawLine(x, ref line, $"Hunter: {snapshot.HunterName ?? "None"}");
        DrawLine(x, ref line, $"Extracted: {snapshot.ExtractedCount}/{snapshot.HeisterCount}");
        DrawLine(x, ref line, "F5 Debug Players / F6 Start / F7 Cash / F8 Extract");
        DrawLine(x, ref line, "F11 Log / F12 Reset / Ctrl+F12 Clear Debug Players");
        DrawLine(x, ref line, "Type digits + Enter = add custom cash / Q = Pulse");
    }

    private static void DrawLine(float x, ref float y, string text)
    {
        GUI.Label(new Rect(x + 12, y, 440, 20), text);
        y += 19;
    }

    private static string FormatTime(float seconds)
    {
        if (seconds < 0f)
        {
            seconds = 0f;
        }

        var totalSeconds = Mathf.CeilToInt(seconds);
        var minutes = totalSeconds / 60;
        var secs = totalSeconds % 60;
        return $"{minutes:00}:{secs:00}";
    }
}
