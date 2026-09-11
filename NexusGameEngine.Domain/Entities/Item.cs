using System.Text.RegularExpressions;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities;

public sealed partial class Item
{

    private const byte MAX_NAME_LENGTH = 50;
    private const byte MAX_DESCRIPTION_LENGTH = 255;

    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public int MaxStackQuantity { get; init; }

    private Item(Guid id, string name, string description, int maxStackQuantity)
    {
        Id = id;
        Name = name;
        Description = description;
        MaxStackQuantity = maxStackQuantity;
    }

    public static Result<Item> Create(string name, string description, int maxStackQuantity)
    {
        if (string.IsNullOrWhiteSpace(name)) return Error.Validation("Item.EmptyName", "The item name cannot be empty");
        if (name.Length > MAX_NAME_LENGTH || name.Length <= 3) return Error.Validation("Item.InvalidNameLength", "The item name length must be between 3 and 50 chars");
        if (!ItemNameRegex().IsMatch(name)) return Error.Validation("Item.InvalidName", "The item name string is invalid");

        if (string.IsNullOrWhiteSpace(description)) return Error.Validation("Item.EmptyDescription", "The item description cannot be empty");
        if (description.Length > MAX_DESCRIPTION_LENGTH || description.Length < 5) return Error.Validation("Item.InvalidDescriptionLength", "The item description must be between 5 and 255 chars");
        if (!ItemDescriptionRegex().IsMatch(description)) return Error.Validation("Item.InvalidDescription", "The item description string is invalid");

        if (maxStackQuantity <= 0) return Error.Validation("Item.InvalidMaxStackQuantity", "The item MaxStackQuantity cannot be 0 or a negative number");

        return new Item(Guid.NewGuid(), name, description, maxStackQuantity);
    }

    [GeneratedRegex(@"^(?=.*\p{L})[\p{L}0-9\s\-\']+$")]
    private static partial Regex ItemNameRegex();

    [GeneratedRegex(@"^(?=.*\p{L})[\p{L}0-9\s\-\'\.,!?:()]+$")]
    private static partial Regex ItemDescriptionRegex();


}
