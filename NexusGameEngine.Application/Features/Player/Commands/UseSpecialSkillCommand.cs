using FluentValidation;
using MediatR;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public record UseSpecialSkillCommand(Guid PlayerId) : IRequest<Result<bool>>;

public class UseSpecialSkillCommandValidator : AbstractValidator<UseSpecialSkillCommand>
{
    public UseSpecialSkillCommandValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("Player Id cannot be empty");
    }
}