using Microsoft.EntityFrameworkCore;

namespace NexusGameEngine.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Player> Players { get; }
    DbSet<Item> Items { get; }
    DbSet<InventorySlot> InventorySlots { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
