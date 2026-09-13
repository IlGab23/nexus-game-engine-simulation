using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public class RemoveInventoryItemHandler(IApplicationDbContext appDbContext, IPlayerLockService playerLockService) : IRequestHandler<RemoveInventoryItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // 1. GET THE LOCK: Retrieve the async semaphore dedicated to this specific PlayerId.
        SemaphoreSlim _lock = playerLockService.GetPlayerLock(request.PlayerId);

        // 2. ACQUIRE THE LOCK: Wait for our turn. No other thread can modify 
        // this player's inventory until we pass this line.
        await _lock.WaitAsync(cancellationToken);

        try
        {
            // 3. LOAD DATA (Entity Framework Core)
            // Load the Player including their inventory slots
            var player = await appDbContext.Players.Include(p => p.InventorySlots)
                            .FirstOrDefaultAsync(p => p.Id == request.PlayerId, cancellationToken);
            if (player is null) return Error.NotFound("RemoveItem.PlayerNotFound", "Player does not exists");

            // Load the Item from the database
            var item = await appDbContext.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);
            if (item is null) return Error.NotFound("RemoveItem.ItemNotFound", "Item does not exists");

            // 4. BUSINESS LOGIC (Domain)
            // Check if the player have slots of that item in his inventory
            List<InventorySlot> itemSlots = [.. player.InventorySlots.Where(iSlot => iSlot.ItemId == item.Id)];
            if (itemSlots.Count == 0) return Error.NotFound("Inventory.NoItemFound", $"Player does not have any {item.Name} in his inventory");

            int totalItemAmount = itemSlots.Sum(item => item.Quantity);
            if (request.Amount > totalItemAmount) return Error.Conflict("RemoveItem.LessItemInInventory", $"Player does not have enough {item.Name} in his inventory");

            int amountToRemove = request.Amount;
            for (int i = 0; i < itemSlots.Count; i++)
            {
                int slotQuantity = itemSlots[i].Quantity;
                int subtracted = Math.Min(amountToRemove, slotQuantity);
                if (subtracted == slotQuantity)
                {
                    var removeResult = player.RemoveInventorySlot(itemSlots[i]);
                    if (removeResult.IsFailure) return removeResult.ErrorList;
                    appDbContext.InventorySlots.Remove(itemSlots[i]);
                    amountToRemove -= subtracted;

                    if (amountToRemove <= 0) break;

                    continue;
                }

                var removeQntResult = itemSlots[i].RemoveQuantity(amountToRemove);
                if (removeQntResult.IsFailure) return removeQntResult.ErrorList;
                break;
            }


            // 5. SAVE TO DATABASE
            // Entity Framework will detect changes made in RAM to tracked objects
            // and execute the necessary INSERTs or UPDATEs in a single transaction.

            await appDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        finally
        {
            // 6. RELEASE THE LOCK: This is always executed, whether on success or exception,
            // allowing the next request for this player to be processed.
            _lock.Release();
        }
    }

}
