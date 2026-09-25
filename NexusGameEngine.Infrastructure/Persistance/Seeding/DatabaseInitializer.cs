using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Application.Interfaces.Security;
using NexusGameEngine.Domain.Constants;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Domain.Entities.ValueObjects;

namespace NexusGameEngine.Infrastructure.Persistance.Seeding;

public class DatabaseInitializer(ApplicationDbContext appDbContext, IPasswordHasher passwordHasher, ILogger<DatabaseInitializer> logger, IConfiguration config) : IDatabaseInitializer
{

    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Initializing database... Applying pending migrations.");
        await appDbContext.Database.MigrateAsync(cancellationToken);
        logger.LogInformation("Database migrations applied successfully. Starting data seeding...");

        await SeedAsync(cancellationToken);
        logger.LogInformation("Database initialization completed successfully.");
    }

    public async Task StoreRoleIds(CancellationToken cancellationToken)
    {
        var baseRoles = SystemRoleNames.GetAllRoles().ConvertAll(r => r.Name);
        var roleIds = await appDbContext.SystemRoles
                            .AsNoTracking()
                            .Where(r => baseRoles.Contains(r.Name))
                            .Select(r => new { r.Name, r.Id })
                            .ToListAsync(cancellationToken);

        if (roleIds.Count < baseRoles.Count)
        {
            logger.LogWarning("System roles check failed: Expected {ExpectedCount} roles but found {ActualCount} in the database. Role IDs will not be cached.", baseRoles.Count, roleIds.Count);
            return;
        }

        Guid adminId = roleIds.First(r => r.Name == SystemRoleNames.Admin.Name).Id;
        Guid playerPlusId = roleIds.First(r => r.Name == SystemRoleNames.PlayerPlus.Name).Id;
        Guid playerId = roleIds.First(r => r.Name == SystemRoleNames.Player.Name).Id;

        SystemRoleNames.SetRoleId(adminId, playerPlusId, playerId);
        logger.LogInformation("System role IDs successfully loaded and cached in memory.");
    }


    public async Task SeedAsync(CancellationToken cancellationToken)
    {

        var seedingRoles = SystemRoleNames.GetAllRoles();
        string[] seededRoles = await appDbContext.SystemRoles
                                    .AsNoTracking()
                                    .Where(r => seedingRoles.Select(sr => sr.Name).Contains(r.Name))
                                    .Select(r => r.Name)
                                    .ToArrayAsync(cancellationToken);
        seedingRoles.RemoveAll(sr => seededRoles.Contains(sr.Name));


        if (seedingRoles.Count > 0)
        {
            logger.LogInformation("Found {MissingRolesCount} missing system roles. Seeding roles...", seedingRoles.Count);
            List<SystemRole> rolesToAdd = new();

            foreach (var (Name, Description, IsAdminRole) in seedingRoles)
            {
                logger.LogInformation("Creating system role: {RoleName}", Name);
                rolesToAdd.Add(SystemRole.Create(Name, Description, IsAdminRole).Value);
            }

            await appDbContext.SystemRoles.AddRangeAsync(rolesToAdd, cancellationToken);

            await appDbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Missing system roles successfully seeded into the database.");
        }

        await StoreRoleIds(cancellationToken);

        string AdminUserName = config["DefaultAdmin:Username"] ?? throw new InvalidOperationException("DB Seeding: Missing Admin Username in configuration file");
        if (!await appDbContext.Users.AnyAsync(u => u.UserName == AdminUserName, cancellationToken) && SystemRoleNames.AdminId is not null)
        {
            logger.LogInformation("Default admin user '{AdminUserName}' not found. Seeding admin user...", AdminUserName);
            string AdminEmailString = config["DefaultAdmin:Email"] ?? throw new InvalidOperationException("DB Seeding: Missing Admin Email in configuration file");
            string AdminPassword = config["DefaultAdmin:Password"] ?? throw new InvalidOperationException("DB Seeding: Missing Admin Password in configuration file");

            string passwordHash = await passwordHasher.HashPassword(AdminPassword);

            Email AdminEmail = Email.Create(AdminEmailString).Value;
            User AdminUser = User.Create(AdminUserName, AdminEmail, passwordHash, SystemRoleNames.AdminId).Value;
            await appDbContext.Users.AddAsync(AdminUser, cancellationToken);
            await appDbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Default admin user '{AdminUserName}' successfully created and assigned Admin role.", AdminUserName);
        }

        await SeedMonsters(cancellationToken);
    }

    private async Task SeedMonsters(CancellationToken cancellationToken)
    {
        List<MonsterTemplate> initialMonsters = new();

        var slimeLoot = new
        {
            XP = 15,
            Money = 5,
            Drops = new[]{
                new {ItemId = Guid.Empty, DropChance = 0.5f} //TODO: Change the id with a real id(adding more items if necessary)
            }
        };
        string slimePayload = JsonSerializer.Serialize(slimeLoot);
        var slime = MonsterTemplate.Create("Green Slime", 30, 5, 1.5f, slimePayload, false).Value;
        initialMonsters.Add(slime);

        var goblinLoot = new
        {
            XP = 30,
            Money = 10,
            Drops = new[]{
                new {ItemId = Guid.Empty, DropChance = 0.5f} //TODO: Change the id with a real id(adding more items if necessary)
            }
        };
        string goblinPayload = JsonSerializer.Serialize(goblinLoot);
        var goblin = MonsterTemplate.Create("Goblin Predone", 60, 12, 2.0f, goblinPayload, false).Value;
        initialMonsters.Add(goblin);

        var wolfLoot = new
        {
            XP = 50,
            Money = 23,
            Drops = new[]{
                new {ItemId = Guid.Empty, DropChance = 0.5f}, //TODO: Change the id with a real id(adding more items if necessary)
                new {ItemId = Guid.Empty, DropChance = 0.5f} //TODO: Change the id with a real id(adding more items if necessary)
            }
        };
        string wolfPayload = JsonSerializer.Serialize(wolfLoot);
        var wolf = MonsterTemplate.Create("Lupo Crudele", 80, 18, 2.5f, wolfPayload, false).Value;
        initialMonsters.Add(wolf);

        var orcLoot = new
        {
            XP = 132,
            Money = 64,
            Drops = new[]{
                new {ItemId = Guid.Empty, DropChance = 0.5f}, //TODO: Change the id with a real id(adding more items if necessary)
                new {ItemId = Guid.Empty, DropChance = 0.5f}, //TODO: Change the id with a real id(adding more items if necessary)
                new {ItemId = Guid.Empty, DropChance = 0.5f} //TODO: Change the id with a real id(adding more items if necessary)
            }
        };
        string orcPayload = JsonSerializer.Serialize(orcLoot);
        var orc = MonsterTemplate.Create("Orco Guerriero", 250, 35, 3.0f, orcPayload, false).Value;
        initialMonsters.Add(orc);

        var specterLoot = new
        {
            XP = 84,
            Money = 45,
            Drops = new[]{
                new {ItemId = Guid.Empty, DropChance = 0.5f}, //TODO: Change the id with a real id(adding more items if necessary)
                new {ItemId = Guid.Empty, DropChance = 0.5f} //TODO: Change the id with a real id(adding more items if necessary)
            }
        };
        string specterPayload = JsonSerializer.Serialize(specterLoot);
        var specter = MonsterTemplate.Create("Spettro Minore", 45, 25, 10.0f, specterPayload, false).Value;
        initialMonsters.Add(specter);

        var bossLoot = new
        {
            XP = 1474,
            Money = 2000,
            Drops = new[]{
                new {ItemId = Guid.Empty, DropChance = 0.5f}, //TODO: Change the id with a real id(adding more items if necessary)
                new {ItemId = Guid.Empty, DropChance = 0.5f}, //TODO: Change the id with a real id(adding more items if necessary)
                new {ItemId = Guid.Empty, DropChance = 0.5f}, //TODO: Change the id with a real id(adding more items if necessary)
                new {ItemId = Guid.Empty, DropChance = 0.5f}, //TODO: Change the id with a real id(adding more items if necessary)
                new {ItemId = Guid.Empty, DropChance = 0.5f} //TODO: Change the id with a real id(adding more items if necessary)
            }
        };
        string bossPayload = JsonSerializer.Serialize(bossLoot);
        var boss = MonsterTemplate.Create("Re dei Goblin (BOSS)", 1500, 60, 4.0f, bossPayload, true).Value;
        initialMonsters.Add(boss);

        var existingNames = await appDbContext.MonsterTemplates
            .Select(m => m.Name)
            .ToListAsync(cancellationToken);

        var monstersToAdd = initialMonsters
            .Where(m => !existingNames.Contains(m.Name))
            .ToList();

        if (monstersToAdd.Any())
        {
            await appDbContext.MonsterTemplates.AddRangeAsync(monstersToAdd, cancellationToken);
            await appDbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("{Count} new monsters successfully seeded into the database.", monstersToAdd.Count);
        }
    }
}
