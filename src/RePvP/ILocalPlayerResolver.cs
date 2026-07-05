using UnityEngine;

namespace RePvP;

public interface ILocalPlayerResolver
{
    GameObject? GetLocalPlayerObject();
    bool IsLocalPlayer(PlayerRef player);
}
