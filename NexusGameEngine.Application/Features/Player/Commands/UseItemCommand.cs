using FluentValidation;
using MediatR;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public record UseItemCommand(Guid PlayerId, Guid InventorySlotId) : IRequest<Result<bool>>;

public class UseItemCommandValidator : AbstractValidator<UseItemCommand>
{
    public UseItemCommandValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("PlayerId Is not valid or empty");

        RuleFor(x => x.InventorySlotId)
            .NotEmpty().WithMessage("InventorySlotId is not valid or empty");
    }
}
