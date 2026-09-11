using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities;

public sealed class InventorySlot
{
    public Guid Id { get; init; }
    public Guid PlayerId { get; init; }
    public Player? Player { get; private set; }
    public Guid ItemId { get; init; }
    public Item? Item { get; private set; }
    public int Quantity { get; private set; }

    private InventorySlot(Guid id, Guid playerId, Guid itemId, int quantity)
    {
        Id = id;
        PlayerId = playerId;
        ItemId = itemId;
        Quantity = quantity;
    }

    public static Result<InventorySlot> Create(Guid playerId, Item item, int quantity)
    {
        if (item is null) return Error.Validation("InventorySlot.InvalidItem", "The item is not defined");

        if (playerId == Guid.Empty) return Error.Validation("InventorySlot.InvalidPlayerId", "Player ID is not valid.");
        if (item.Id == Guid.Empty) return Error.Validation("InventorySlot.InvalidItemId", "Item ID is not valid.");

        if (quantity <= 0) return Error.Validation("InventorySlot.InvalidQuantity", "Quantity cannot be 0 or less");
        if (quantity > item.MaxStackQuantity) return Error.Validation("InventorySlot.InvalidQuantity", $"Quantity cannot be more than the MaxStackQuantity({item.MaxStackQuantity}) of the item");

        return new InventorySlot(Guid.NewGuid(), playerId, item.Id, quantity);
    }

    public Result<bool> AddQuantity(int amount, int maxStackQuantity)
    {
        if (amount <= 0) return Error.Validation("InventorySlot.InvalidAddQuantity", "The quantity to add cannot be 0 or less");

        // We cast to long before adding to prevent integer overflow exploits. 
        // If 'amount' is int.MaxValue, adding it to an int Quantity would wrap around to a negative number,
        // maliciously bypassing the > maxStackQuantity validation.
        long tempQnt = (long)Quantity + amount;
        if (tempQnt > maxStackQuantity) return Error.Validation("InventorySlot.InvalidAddQuantity", $"The quantity cannot be more than the MaxStackQuantity({maxStackQuantity}) of the item");

        Quantity = (int)tempQnt;
        return true;
    }

    public Result<bool> RemoveQuantity(int amount)
    {

        if (amount <= 0) return Error.Validation("InventorySlot.InvalidRemoveQuantity", "The quantity to remove cannot be 0 or less");
        if (Quantity < amount) return Error.Validation("InventorySlot.InvalidRemoveQuantity", "The quantity cannot be 0 or less");

        Quantity -= amount;
        return true;
    }
}
