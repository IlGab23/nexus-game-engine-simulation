using System.Data;
using FluentValidation;
using MediatR;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public record MovePlayerCommand(Guid PlayerId, float TargetX, float TargetY, float TargetZ) : IRequest<Result<bool>>;

public class MovePlayerCommandValidator : AbstractValidator<MovePlayerCommand>
{
    public MovePlayerCommandValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("PlayerId cannot be empty");

        RuleFor(x => x.TargetX)
            .InclusiveBetween(0, 3000).WithMessage("TargetX cannot be less than 0 or more than 3000");

        RuleFor(x => x.TargetY)
            .InclusiveBetween(0, 3000).WithMessage("TargetY cannot be less than 0 or more than 3000");

        RuleFor(x => x.TargetZ)
            .InclusiveBetween(-1000, 1000).WithMessage("TargetZ cannot be less than -1000 or more than 1000");
        // -1000 Z is the max height that the player cannot go under that
        // because map will have underground sections so Z can go under 0 but
        // there is a cap either to prevent player to fall into the map




    }
}
