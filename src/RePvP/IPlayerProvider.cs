using System.Collections.Generic;

namespace RePvP;

public interface IPlayerProvider
{
    IReadOnlyList<PlayerRef> GetPlayers();
}
