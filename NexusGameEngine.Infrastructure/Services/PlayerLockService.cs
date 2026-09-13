using System.Collections.Concurrent;
using NexusGameEngine.Application.Interfaces;

namespace NexusGameEngine.Infrastructure.Services;

public class PlayerLockService : IPlayerLockService
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();

    public SemaphoreSlim GetPlayerLock(Guid playerId)
    {
        return _locks.GetOrAdd(playerId, _ => new SemaphoreSlim(1, 1));
    }

}
