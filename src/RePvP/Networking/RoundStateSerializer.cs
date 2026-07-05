using System.Globalization;
using System.Text;

namespace RePvP;

public static class RoundStateSerializer
{
    public static string ToJson(RoundStatePacket packet)
    {
        var builder = new StringBuilder();
        builder.Append('{');
        AppendString(builder, "phase", packet.Phase);
        builder.Append(',');
        AppendInt(builder, "currentCash", packet.CurrentCash);
        builder.Append(',');
        AppendInt(builder, "requiredCash", packet.RequiredCash);
        builder.Append(',');
        AppendFloat(builder, "phaseTimeRemaining", packet.PhaseTimeRemaining);
        builder.Append(',');
        AppendFloat(builder, "roundTimeRemaining", packet.RoundTimeRemaining);
        builder.Append(',');
        AppendInt(builder, "heisterCount", packet.HeisterCount);
        builder.Append(',');
        AppendInt(builder, "extractedCount", packet.ExtractedCount);
        builder.Append(',');
        AppendString(builder, "hunterName", packet.HunterName);
        builder.Append(',');
        AppendFloat(builder, "cashProgress", packet.CashProgress);
        builder.Append('}');
        return builder.ToString();
    }

    public static string ToJson(RoundSnapshot snapshot)
    {
        return ToJson(RoundStatePacket.FromSnapshot(snapshot));
    }

    private static void AppendString(StringBuilder builder, string name, string value)
    {
        builder.Append('"').Append(name).Append("\":");
        builder.Append('"').Append(Escape(value)).Append('"');
    }

    private static void AppendInt(StringBuilder builder, string name, int value)
    {
        builder.Append('"').Append(name).Append("\":");
        builder.Append(value.ToString(CultureInfo.InvariantCulture));
    }

    private static void AppendFloat(StringBuilder builder, string name, float value)
    {
        builder.Append('"').Append(name).Append("\":");
        builder.Append(value.ToString("0.###", CultureInfo.InvariantCulture));
    }

    private static string Escape(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}
