using FluentValidation;
using MediatR;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public record AddInventoryItemCommand(Guid PlayerId, Guid ItemId, int Amount) : IRequest<Result<bool>>;

public sealed class AddInventoryItemCommandValidator : AbstractValidator<AddInventoryItemCommand>
{
    public AddInventoryItemCommandValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("PlayerId cannot be empty");

        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("ItemId cannot be empty");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount cannot be 0 or less");
    }
}

