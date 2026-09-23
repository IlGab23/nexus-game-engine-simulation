using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Moq;
using NexusGameEngine.Application.Features.Player.Commands;
using NexusGameEngine.Domain.Constants;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Domain.Entities.ValueObjects;
using NexusGameEngine.Domain.Enums;
using NexusGameEngine.Infrastructure.Persistance;
using Xunit;
using static System.Threading.CancellationToken;

namespace NexusGameEngineApplication.Tests.IntegrationTests;

public class UseItemHandlerTests
{
    [Fact]
    public async Task Handle_WithConsumableItem_ShouldApplyEffectsAndReduceQuantity()
    {
        // 1. Arrange - Setup del Database In-Memory SQLite
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseSqlite("DataSource=:memory:")
                            .Options;
        using var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();
        var fakeTimeProvider = new FakeTimeProvider();
        var nowTime = fakeTimeProvider.GetUtcNow();

        //Arrange - Seeding
        var emailResult = Email.Create("testUser@gmail.com");
        var userResult = User.Create("testUser", emailResult.Value, "HashedPassword", SystemRoleNames.PlayerId);
        var user = userResult.Value;

        var player = Player.Create(user.Id, nowTime).Value;

        player.TakeDamage(20);
        player.ConsumeStamina(15, nowTime);

        int initialHealth = player.PlayerHealth.CurrentHealth;
        short initialStamina = player.PlayerStamina.CurrentStamina;

        string potionPayload = JsonSerializer.Serialize(new { HealAmount = 20, StaminaAmount = 10 });
        var item = Item.Create("Super Magic Potion", "Heal and give stamina", 10, ItemType.Consumable, potionPayload).Value;

        var invSlot = InventorySlot.Create(player.Id, item, 2, "{}").Value;
        player.AddInventorySlot(invSlot);

        dbContext.Users.Add(user);
        dbContext.Players.Add(player);
        dbContext.Items.Add(item);
        await dbContext.SaveChangesAsync();

        // Setup handler and command
        var handler = new UseItemHandler(dbContext, fakeTimeProvider);
        var command = new UseItemCommand(player.Id, invSlot.Id);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var playerInDb = await dbContext.Players.Include(p => p.InventorySlots)
                                                .FirstAsync(p => p.Id == player.Id);

        playerInDb.PlayerHealth.CurrentHealth.Should().Be((short)(initialHealth + 20));
        playerInDb.PlayerStamina.CurrentStamina.Should().Be((short)(initialStamina + 10));

        var updatedSlot = playerInDb.InventorySlots.First(s => s.Id == invSlot.Id);
        updatedSlot.Quantity.Should().Be(1);

        result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        playerInDb = await dbContext.Players.Include(p => p.InventorySlots)
                                            .FirstAsync(p => p.Id == player.Id);

        playerInDb.PlayerHealth.CurrentHealth.Should().Be((short)(player.PlayerHealth.MaxHealth));
        playerInDb.PlayerStamina.CurrentStamina.Should().Be((short)(player.PlayerStamina.MaxStamina));

        updatedSlot = playerInDb.InventorySlots.FirstOrDefault(s => s.Id == invSlot.Id);
        updatedSlot.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithConsumableItem_ShouldReturnFalseCausePlayerIsDead()
    {
        // 1. Arrange - Setup del Database In-Memory SQLite
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseSqlite("DataSource=:memory:")
                            .Options;
        using var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();
        var fakeTimeProvider = new FakeTimeProvider();
        var nowTime = fakeTimeProvider.GetUtcNow();

        //Arrange - Seeding
        var emailResult = Email.Create("testUser@gmail.com");
        var userResult = User.Create("testUser", emailResult.Value, "HashedPassword", SystemRoleNames.PlayerId);
        var user = userResult.Value;

        var player = Player.Create(user.Id, nowTime).Value;

        player.TakeDamage(9999);
        player.ConsumeStamina(15, nowTime);

        string potionPayload = JsonSerializer.Serialize(new { HealAmount = 20, StaminaAmount = 10 });
        var item = Item.Create("Super Magic Potion", "Heal and give stamina", 10, ItemType.Consumable, potionPayload).Value;

        var invSlot = InventorySlot.Create(player.Id, item, 2, "{}").Value;
        player.AddInventorySlot(invSlot);

        dbContext.Users.Add(user);
        dbContext.Players.Add(player);
        dbContext.Items.Add(item);
        await dbContext.SaveChangesAsync();

        // Setup handler and command
        var handler = new UseItemHandler(dbContext, fakeTimeProvider);
        var command = new UseItemCommand(player.Id, invSlot.Id);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();

        var playerInDb = await dbContext.Players.FirstAsync(p => p.Id == player.Id);
        playerInDb.IsAlive.Should().BeFalse();
    }
}
