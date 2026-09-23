using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Features.Player.Commands;
using NexusGameEngine.Domain.Constants;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Domain.Entities.ValueObjects;
using NexusGameEngine.Domain.Enums;
using NexusGameEngine.Domain.ResultPattern;
using NexusGameEngine.Infrastructure.Persistance;
using NexusGameEngine.Infrastructure.Services;
using Xunit;

namespace NexusGameEngineApplication.Tests.IntegrationTests;

public class AddInventoryItemConcurrencyTests
{
    [Fact]
    public async Task Handle_WithConcurrentRequests_ShouldNotDuplicateItems()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseSqlite("DataSource=:memory:")
                            .Options;

        using var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.OpenConnectionAsync();

        //NOTE: This will crash until we will make entity configurations in [INV-04] task
        await dbContext.Database.EnsureCreatedAsync();

        //Seeding: Creating User - Player entity and item entity

        var emailResult = Email.Create("testUser@gmail.com");
        var userResult = User.Create("testUser", emailResult.Value, "HashedPassword", SystemRoleNames.PlayerId);
        var user = userResult.Value;

        var playerResult = Player.Create(user.Id, TimeProvider.System.GetUtcNow());
        var player = playerResult.Value;

        var itemResult = Item.Create("Pozione Magica", "Una pozione magica di magia magica", 20, ItemType.Misc, "{}");
        var item = itemResult.Value;

        //Adding entities to DB
        dbContext.Users.Add(user);
        dbContext.Players.Add(player);
        dbContext.Items.Add(item);
        await dbContext.SaveChangesAsync();

        //Preparing lock, handler, command
        var lockService = new PlayerLockService();
        var handler = new AddInventoryItemHandler(dbContext, lockService);
        var command = new AddInventoryItemCommand(player.Id, item.Id, 1); // This command will try to add 1 item per time

        //Attack
        int numberOfConcurrentRequests = 50;
        var tasks = new List<Task<Result<bool>>>();

        for (int i = 0; i < numberOfConcurrentRequests; i++)
        {
            tasks.Add(Task.Run(() => handler.Handle(command, CancellationToken.None)));
        }

        Result<bool>[] allResults = await Task.WhenAll(tasks);

        var playerInDb = await dbContext.Players
                        .Include(p => p.InventorySlots)
                        .FirstAsync(p => p.Id == player.Id);

        // 1. Verify that exactly 3 slots were created (Multi-slotting behavior)
        playerInDb.InventorySlots.Should().HaveCount(3);

        // 2. Verify that the total sum of potions is EXACTLY 50 (No items lost or duplicated)
        playerInDb.InventorySlots.Sum(s => s.Quantity).Should().Be(50);

        // 3. Verify that the MaxStackQuantity(20) logic filled exactly 2 slots to their maximum capacity
        playerInDb.InventorySlots.Count(s => s.Quantity == 20).Should().Be(2);

        // 4. Verify that the remainder (10 potions) ended up in the last partial slot
        playerInDb.InventorySlots.Count(s => s.Quantity == 10).Should().Be(1);

        //5. Verify that all request results contains isSuccess = true
        allResults.Should().OnlyContain(result => result.IsSuccess, "All 50 requests return isSuccess = true without errors");

    }
}
