namespace RePvP;

public static class ValuableCashInBridge
{
    public static void OnValuableCashed(int cashValue, object? source = null)
    {
        // Intended hook point for the real R.E.P.O. cash-in event.
        // Once the actual method/class is identified, call this method from a Harmony postfix/prefix.
        if (cashValue <= 0)
        {
            return;
        }

        Plugin.Log.LogInfo($"Valuable cashed via bridge: ${cashValue:N0}. Source: {source?.GetType().Name ?? "unknown"}");
        RePvPApi.NotifyValuableCashed(cashValue);
    }
}
