using UnityEngine;

namespace RePvP;

public sealed class ExtractionZone : MonoBehaviour
{
    [SerializeField]
    private bool _logNonPlayerEntries = false;

    private void Reset()
    {
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
        {
            return;
        }

        var extracted = RePvPApi.NotifyPlayerEnteredExtraction(other.gameObject);
        if (!extracted && _logNonPlayerEntries)
        {
            Plugin.Log.LogInfo($"ExtractionZone ignored object: {other.gameObject.name}");
        }
    }
}
