namespace NexusGameEngine.Domain.Entities.Payloads;

public record MiscPayload(bool? IsQuestItem, string? RelatedQuestId, int? SellValue);
