using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities;

public class MapZone
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public int MaxPlayers { get; private set; }

    private MapZone(Guid id, string name, int maxPlayers)
    {
        Id = id;
        Name = name;
        MaxPlayers = maxPlayers;
    }

    public Result<MapZone> Create(string name, int maxPlayers)
    {
        if (string.IsNullOrWhiteSpace(name)) return Error.Validation("MapZone.InvalidName", "The map name cannot be empty");
        if (maxPlayers <= 0) return Error.Validation("MapZone.InvalidMaxPlayersNumber", "The max number of player for the map cannot be 0 or less");

        return new MapZone(Guid.NewGuid(), name, maxPlayers);
    }
}
