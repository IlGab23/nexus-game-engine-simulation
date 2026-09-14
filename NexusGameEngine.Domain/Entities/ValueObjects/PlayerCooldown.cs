using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities.ValueObjects;

public record PlayerCooldown
{
    public string ActionId { get; init; }
    public DateTimeOffset ReadyAt { get; init; }

    private PlayerCooldown(string actioId, DateTimeOffset readyAt)
    {
        ActionId = actioId;
        ReadyAt = readyAt;
    }

    public static Result<PlayerCooldown> Create(string actionId, DateTimeOffset readyAt, DateTimeOffset currentTime)
    {
        if (string.IsNullOrWhiteSpace(actionId)) return Error.Validation("PlayerCooldown.ActionIdEmpty", "ActionId cannot be empty");
        if (readyAt == DateTimeOffset.MinValue) return Error.Validation("PlayerCooldown.ReadyAtEmpty", "The ReadyAt date cannot be left empty");
        if (readyAt <= currentTime) return Error.Validation("PlayerCooldown.ReadyAtInThePast", "The ReadyAt date cannot be in the past");

        return new PlayerCooldown(actionId, readyAt);
    }

}
