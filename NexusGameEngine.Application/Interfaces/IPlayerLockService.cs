
namespace NexusGameEngine.Application.Interfaces;

public interface IPlayerLockService
{
    SemaphoreSlim GetPlayerLock(Guid playerId);
}
