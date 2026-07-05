using UnityEngine;

namespace RePvP;

public static class RePvPApi
{
    public static GamePhase CurrentPhase => Plugin.RoundManager.Phase;
    public static RoundSnapshot Snapshot => Plugin.RoundManager.GetSnapshot();

    public static void NotifyValuableCashed(int cashValue)
    {
        if (cashValue <= 0)
        {
            return;
        }

        Plugin.RoundManager.AddCash(cashValue);
    }

    public static bool NotifyPlayerEnteredExtraction(GameObject playerObject)
    {
        return Plugin.RoundManager.TryMarkHeisterExtracted(playerObject);
    }

    public static void ForceStartRound()
    {
        Plugin.RoundManager.ForceStartRound();
    }

    public static void ForceExtraction()
    {
        Plugin.RoundManager.TriggerExtractionForDebug();
    }
}
