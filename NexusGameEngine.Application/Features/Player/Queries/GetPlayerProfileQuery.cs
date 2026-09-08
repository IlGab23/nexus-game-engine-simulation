using FluentValidation;
using MediatR;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Queries;

public record GetPlayerProfileQuery(Guid PlayerId) : IRequest<Result<GetPlayerProfileOutput>>;

public record GetPlayerProfileOutput(
    Guid Id,
    StatDto MainLevel,
    StatDto Strength,
    StatDto Dexterity,
    StatDto Intelligence,
    StatDto Constitution,
    HealthDto Health,
    StaminaDto Stamina,
    int Money,
    bool IsAlive
);

public record StatDto(string Name, int Experience, byte Level, int ExperienceCapForNextLevel);
public record HealthDto(short CurrentHealth, short MaxHealth);
public record StaminaDto(short ActualStamina, short MaxStamina, short RegenRatePerSecond);

public sealed class GetPlayerProfileQueryValidator : AbstractValidator<GetPlayerProfileQuery>
{
    public GetPlayerProfileQueryValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("No PlayerId Provided");
    }
}