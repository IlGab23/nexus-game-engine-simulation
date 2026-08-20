namespace NexusGameEngine.Infrastructure.Persistance.Seeding;

public interface IDatabaseInitializer
{
    Task SeedAsync(CancellationToken cancellationToken);
    Task StoreRoleIds(CancellationToken cancellationToken);
    Task InitializeDatabaseAsync(CancellationToken cancellationToken);
}
