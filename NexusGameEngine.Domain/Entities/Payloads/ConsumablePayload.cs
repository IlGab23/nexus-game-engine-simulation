namespace NexusGameEngine.Domain.Entities.Payloads;

public record ConsumablePayload(int? HealAmount, int? StaminaAmount, int? DurationInSeconds);
