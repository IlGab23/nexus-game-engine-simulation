using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Moq;
using NexusGameEngine.Application.Features.Player.Commands;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Infrastructure.Persistance;
using Xunit;

namespace NexusGameEngineApplication.Tests.UnitTests;

public class ClaimDailyRewardHandlerTests
{
    [Fact]
    public async Task Handle_WithAvailableReward_ShouldReturnSuccessWithMoney()
    {
        // Arrange

        var fakeRandomProvider = new Mock<IRandomProvider>();
        fakeRandomProvider.Setup(rndp => rndp.GetRandomNumberInRange(It.IsAny<int>(), It.IsAny<int>()))
                            .Returns(20);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        // NOTE: EnsureCreatedAsync crasherà finché non completiamo le configurazioni EF Core in [INV-04]
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var fakeTimeProvider = new FakeTimeProvider();
        var nowTime = fakeTimeProvider.GetUtcNow();

        var player = Player.Create(Guid.NewGuid(), nowTime).Value;
        dbContext.Players.Add(player);
        int beforePlayerMoney = player.Money;

        await dbContext.SaveChangesAsync();

        var firstHandler = new ClaimDailyRewardHandler(dbContext, fakeTimeProvider, fakeRandomProvider.Object);
        var firstCommand = new ClaimDailyRewardCommand(player.Id);

        var secondHandler = new ClaimDailyRewardHandler(dbContext, fakeTimeProvider, fakeRandomProvider.Object);
        var secondCommand = new ClaimDailyRewardCommand(player.Id);

        var thirdHandler = new ClaimDailyRewardHandler(dbContext, fakeTimeProvider, fakeRandomProvider.Object);
        var thirdCommand = new ClaimDailyRewardCommand(player.Id);

        // Act
        var firstResult = await firstHandler.Handle(firstCommand, CancellationToken.None);

        int afterFirstHandlerMoney = await dbContext.Players.Where(p => p.Id == player.Id).Select(p => p.Money).FirstAsync();

        var secondResult = await secondHandler.Handle(secondCommand, CancellationToken.None);

        int afterSecondHandlerMoney = await dbContext.Players.Where(p => p.Id == player.Id).Select(p => p.Money).FirstAsync();

        fakeTimeProvider.Advance(TimeSpan.FromHours(24));
        var thirdResult = await thirdHandler.Handle(thirdCommand, CancellationToken.None);

        int finalMoney = await dbContext.Players.Where(p => p.Id == player.Id).Select(p => p.Money).FirstAsync();

        // Assert

        firstResult.IsSuccess.Should().BeTrue();
        secondResult.IsFailure.Should().BeTrue();
        thirdResult.IsSuccess.Should().BeTrue();

        afterFirstHandlerMoney.Should().BeGreaterThan(beforePlayerMoney);
        afterSecondHandlerMoney.Should().Be(afterFirstHandlerMoney);
        finalMoney.Should().BeGreaterThan(afterSecondHandlerMoney);


        var playerWithCooldowns = await dbContext.Players.Include(p => p.Cooldowns).FirstAsync();
        playerWithCooldowns.Cooldowns.Should().HaveCount(1);
    }


    [Fact]
    public async Task Handle_WithAvailableReward_ShouldReturnSuccessWIthItem()
    {
        // Arrange

        var fakeRandomProvider = new Mock<IRandomProvider>();
        fakeRandomProvider.Setup(rndp => rndp.GetRandomNumberInRange(It.IsAny<int>(), It.IsAny<int>()))
                            .Returns(80);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        // NOTA: EnsureCreatedAsync crasherà finché non completiamo le configurazioni EF Core in [INV-04]
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var fakeTimeProvider = new FakeTimeProvider();
        var nowTime = fakeTimeProvider.GetUtcNow();

        var player = Player.Create(Guid.NewGuid(), nowTime).Value;
        dbContext.Players.Add(player);
        int playerInvSlotCount = player.InventorySlots.Count;

        var item = Item.Create("Spada Epica", "Spada di test", 1).Value;
        dbContext.Items.Add(item);

        await dbContext.SaveChangesAsync();

        var firstHandler = new ClaimDailyRewardHandler(dbContext, fakeTimeProvider, fakeRandomProvider.Object);
        var firstCommand = new ClaimDailyRewardCommand(player.Id);

        var secondHandler = new ClaimDailyRewardHandler(dbContext, fakeTimeProvider, fakeRandomProvider.Object);
        var secondCommand = new ClaimDailyRewardCommand(player.Id);

        var thirdHandler = new ClaimDailyRewardHandler(dbContext, fakeTimeProvider, fakeRandomProvider.Object);
        var thirdCommand = new ClaimDailyRewardCommand(player.Id);

        // Act
        var firstResult = await firstHandler.Handle(firstCommand, CancellationToken.None);

        var playerAfterFirstHandler = await dbContext.Players.Include(p => p.InventorySlots).FirstAsync();
        int afterFirstHandlerInvSlotCount = playerAfterFirstHandler.InventorySlots.Count;

        var secondResult = await secondHandler.Handle(secondCommand, CancellationToken.None);

        var playerAfterSecondHandler = await dbContext.Players.Include(p => p.InventorySlots).FirstAsync();
        int afterSecondHandlerInvSlotCount = playerAfterSecondHandler.InventorySlots.Count;

        fakeTimeProvider.Advance(TimeSpan.FromHours(24));
        var thirdResult = await thirdHandler.Handle(thirdCommand, CancellationToken.None);

        var playerAfterThirdHandler = await dbContext.Players.Include(p => p.InventorySlots).FirstAsync();
        int afterThirdHandlerInvSlotCount = playerAfterThirdHandler.InventorySlots.Count;

        // Assert

        firstResult.IsSuccess.Should().BeTrue();
        secondResult.IsFailure.Should().BeTrue();
        thirdResult.IsSuccess.Should().BeTrue();

        afterFirstHandlerInvSlotCount.Should().Be(1);
        afterSecondHandlerInvSlotCount.Should().Be(1);
        afterThirdHandlerInvSlotCount.Should().Be(2);

        var playerWithCooldowns = await dbContext.Players.Include(p => p.Cooldowns).FirstAsync();
        playerWithCooldowns.Cooldowns.Should().HaveCount(1);

    }
}
