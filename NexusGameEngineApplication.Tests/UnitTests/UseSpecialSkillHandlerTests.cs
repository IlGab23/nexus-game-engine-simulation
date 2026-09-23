using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using NexusGameEngine.Application.Features.Player.Commands;
using NexusGameEngine.Domain.Constants;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Domain.Entities.ValueObjects;
using NexusGameEngine.Infrastructure.Persistance;
using Xunit;

namespace NexusGameEngineApplication.Tests.UnitTests;

public class UseSpecialSkillHandlerTests
{
    [Fact]
    public async Task Handle_WithSkillAvailable_ShouldReturnSuccess()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        // NOTE: EnsureCreatedAsync crasherà finché non completiamo le configurazioni EF Core in [INV-04]
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var fakeTimeProvider = new FakeTimeProvider();
        var nowTime = fakeTimeProvider.GetUtcNow();

        var emailResult = Email.Create("testUser@gmail.com");
        var userResult = User.Create("testUser", emailResult.Value, "HashedPassword", SystemRoleNames.PlayerId);
        var user = userResult.Value;

        var player = Player.Create(user.Id, nowTime).Value;
        var specialSkill = GameConstants.SpecialSkillsCatalog.ConstitutionLevel2;
        player.EquipSpecialSkill(specialSkill);
        dbContext.Users.Add(user);
        dbContext.Players.Add(player);

        await dbContext.SaveChangesAsync();

        var handlerWithNoCooldown = new UseSpecialSkillHandler(dbContext, fakeTimeProvider);
        var handlerWithCooldown = new UseSpecialSkillHandler(dbContext, fakeTimeProvider);
        var handlerWithCooldownReset = new UseSpecialSkillHandler(dbContext, fakeTimeProvider);
        var command = new UseSpecialSkillCommand(player.Id);

        // Act
        var handlerWithNoCooldownResult = await handlerWithNoCooldown.Handle(command, CancellationToken.None);

        var handlerWithCooldownResult = await handlerWithCooldown.Handle(command, CancellationToken.None);

        fakeTimeProvider.Advance(GameConstants.ActionCooldowns.UseSpecialSkillCooldown + TimeSpan.FromMinutes(1));

        var handlerWithCooldownResetResult = await handlerWithCooldownReset.Handle(command, CancellationToken.None);

        //Assert
        handlerWithNoCooldownResult.IsSuccess.Should().BeTrue();
        handlerWithCooldownResult.IsFailure.Should().BeTrue();
        handlerWithCooldownResetResult.IsSuccess.Should().BeTrue();

        var playerFromDb = await dbContext.Players.FirstAsync();
        playerFromDb.Cooldowns.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_WithSkillUnavailable_ShouldReturnFailure()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        // NOTE: EnsureCreatedAsync crasherà finché non completiamo le configurazioni EF Core in [INV-04]
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var fakeTimeProvider = new FakeTimeProvider();
        var nowTime = fakeTimeProvider.GetUtcNow();

        var emailResult = Email.Create("testUser@gmail.com");
        var userResult = User.Create("testUser", emailResult.Value, "HashedPassword", SystemRoleNames.PlayerId);
        var user = userResult.Value;

        var player = Player.Create(user.Id, nowTime).Value;
        dbContext.Users.Add(user);
        dbContext.Players.Add(player);

        await dbContext.SaveChangesAsync();

        var handler = new UseSpecialSkillHandler(dbContext, fakeTimeProvider);
        var command = new UseSpecialSkillCommand(player.Id);

        // Act
        var handlerResult = await handler.Handle(command, CancellationToken.None);

        //Assert
        handlerResult.IsSuccess.Should().BeFalse();

        var playerFromDb = await dbContext.Players.FirstAsync();
        playerFromDb.Cooldowns.Should().HaveCount(0);
    }
}
