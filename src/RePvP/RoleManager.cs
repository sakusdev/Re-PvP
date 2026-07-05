using System;
using System.Collections.Generic;
using System.Linq;

namespace RePvP;

public sealed class RoleManager
{
    private readonly Dictionary<string, Team> _roles = new();
    private readonly Random _random = new();

    public PlayerRef? Hunter { get; private set; }
    public IReadOnlyList<PlayerRef> Heisters { get; private set; } = Array.Empty<PlayerRef>();

    public Team GetTeam(PlayerRef player)
    {
        return _roles.TryGetValue(player.Id, out var team) ? team : Team.None;
    }

    public void AssignTeams(IReadOnlyList<PlayerRef> players)
    {
        _roles.Clear();
        Hunter = null;
        Heisters = Array.Empty<PlayerRef>();

        if (players.Count == 0)
        {
            Plugin.Log.LogWarning("Cannot assign teams: no players found.");
            return;
        }

        var hunterIndex = _random.Next(players.Count);
        Hunter = players[hunterIndex];

        var heisters = new List<PlayerRef>();
        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            var team = i == hunterIndex ? Team.Hunter : Team.Heisters;
            _roles[player.Id] = team;

            if (team == Team.Heisters)
            {
                heisters.Add(player);
            }
        }

        Heisters = heisters;

        Plugin.Log.LogInfo($"Hunter selected: {Hunter}");
        Plugin.Log.LogInfo($"Heisters: {string.Join(", ", Heisters.Select(h => h.DisplayName))}");
    }

    public void Clear()
    {
        _roles.Clear();
        Hunter = null;
        Heisters = Array.Empty<PlayerRef>();
    }
}
