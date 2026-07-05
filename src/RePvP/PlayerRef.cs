using UnityEngine;

namespace RePvP;

public sealed class PlayerRef
{
    public PlayerRef(string id, string displayName, GameObject gameObject)
    {
        Id = id;
        DisplayName = displayName;
        GameObject = gameObject;
    }

    public string Id { get; }
    public string DisplayName { get; }
    public GameObject GameObject { get; }

    public Vector3 Position => GameObject != null ? GameObject.transform.position : Vector3.zero;

    public override string ToString()
    {
        return $"{DisplayName} ({Id})";
    }
}
