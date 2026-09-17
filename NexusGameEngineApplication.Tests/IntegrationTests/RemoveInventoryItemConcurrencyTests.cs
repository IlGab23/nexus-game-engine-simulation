using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Features.Player.Commands;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Domain.Enums;
using NexusGameEngine.Domain.ResultPattern;
using NexusGameEngine.Infrastructure.Persistance;
using NexusGameEngine.Infrastructure.Services;
using Xunit;

namespace NexusGameEngineApplication.Tests.IntegrationTests;

public class RemoveInventoryItemConcurrencyTests
{
    [Fact]
    public async Task Handle_WithConcurrentRequests_ShouldNotCorruptInventory()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseSqlite("DataSource=:memory:")
                            .Options;

        using var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.OpenConnectionAsync();

        //NOTE: This will crash until we will make entity configurations in [INV-04] task
        await dbContext.Database.EnsureCreatedAsync();

        //Seeding: Creating Player entity and item entity
        var playerResult = Player.Create(Guid.NewGuid(), TimeProvider.System.GetUtcNow());
        var player = playerResult.Value;

        var itemResult = Item.Create("Pozione Magica", "Una pozione magica di magia magica", 20, ItemType.Misc, "{}");
        var item = itemResult.Value;

        //Giving to player 50 potions divided for 3 slots
        player.AddInventorySlot(InventorySlot.Create(player.Id, item, 20, "{}").Value);
        player.AddInventorySlot(InventorySlot.Create(player.Id, item, 20, "{}").Value);
        player.AddInventorySlot(InventorySlot.Create(player.Id, item, 10, "{}").Value);

        //Adding entities to DB
        dbContext.Players.Add(player);
        dbContext.Items.Add(item);
        await dbContext.SaveChangesAsync();

        //Preparing lock, handler, command
        var lockService = new PlayerLockService();
        var handler = new RemoveInventoryItemHandler(dbContext, lockService);
        var command = new RemoveInventoryItemCommand(player.Id, item.Id, 1); // This command will try to remove 1 item per time

        //Attack
        int numberOfConcurrentRequests = 46;
        var tasks = new List<Task<Result<bool>>>();

        for (int i = 0; i < numberOfConcurrentRequests; i++)
        {
            tasks.Add(Task.Run(() => handler.Handle(command, CancellationToken.None)));
        }

        Result<bool>[] allResults = await Task.WhenAll(tasks);

        var playerInDb = await dbContext.Players
                        .Include(p => p.InventorySlots)
                        .FirstAsync(p => p.Id == player.Id);

        // 1. Verify that exactly 1 slot remained
        playerInDb.InventorySlots.Should().HaveCount(1);

        // 2. Verify that the total sum of potions is EXACTLY 4
        playerInDb.InventorySlots.Sum(s => s.Quantity).Should().Be(4);

        //5. Verify that all request results contains isSuccess = true
        allResults.Should().OnlyContain(result => result.IsSuccess, "All 46 requests return isSuccess = true without errors");

    }
}
