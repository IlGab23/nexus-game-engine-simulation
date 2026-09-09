using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Domain.Entities.ValueObjects;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Queries;

public class GetPlayerProfileHandler(IApplicationDbContext appDbContext, TimeProvider timeProvider) : IRequestHandler<GetPlayerProfileQuery, Result<GetPlayerProfileOutput>>
{
    public async Task<Result<GetPlayerProfileOutput>> Handle(GetPlayerProfileQuery request, CancellationToken cancellationToken)
    {
        var player = await appDbContext.Players.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.PlayerId, cancellationToken);
        if (player is null) return Error.NotFound("Player not found", "The player does not exist");

        StatDto mainLevelStatDto = new(
            player.MainLevel.Name,
            player.MainLevel.Experience,
            player.MainLevel.Level,
            player.MainLevel.ExperienceCapForNextLevel);

        StatDto strengthStatDto = new(
            player.Strength.Name,
            player.Strength.Experience,
            player.Strength.Level,
            player.Strength.ExperienceCapForNextLevel);

        StatDto dexterityStatDto = new(
            player.Dexterity.Name,
            player.Dexterity.Experience,
            player.Dexterity.Level,
            player.Dexterity.ExperienceCapForNextLevel);

        StatDto intelligenceStatDto = new(
            player.Intelligence.Name,
            player.Intelligence.Experience,
            player.Intelligence.Level,
            player.Intelligence.ExperienceCapForNextLevel);

        StatDto constitutionStatDto = new(
            player.Constitution.Name,
            player.Constitution.Experience,
            player.Constitution.Level,
            player.Constitution.ExperienceCapForNextLevel);

        HealthDto healthDto = new(
            player.PlayerHealth.CurrentHealth,
            player.PlayerHealth.MaxHealth);

        StaminaDto staminaDto = new(
            player.PlayerStamina.GetActualStamina(timeProvider.GetUtcNow()),
            player.PlayerStamina.MaxStamina,
            player.PlayerStamina.RegenRatePerSecond);

        GetPlayerProfileOutput getPlayerProfileOutput = new(
            request.PlayerId,
            mainLevelStatDto,
            strengthStatDto,
            dexterityStatDto,
            intelligenceStatDto,
            constitutionStatDto,
            healthDto,
            staminaDto,
            player.Money,
            player.IsAlive);

        return getPlayerProfileOutput;
    }

}
