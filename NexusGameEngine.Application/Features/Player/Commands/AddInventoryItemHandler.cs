using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public class AddInventoryItemHandler(IApplicationDbContext appDbContext, IPlayerLockService playerLockService) : IRequestHandler<AddInventoryItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AddInventoryItemCommand request, CancellationToken cancellationToken)
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
            if (player is null) return Error.NotFound("AddItem.PlayerNotFound", "Player does not exists");

            // Load the Item from the database
            var item = await appDbContext.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);
            if (item is null) return Error.NotFound("AddItem.ItemNotFound", "Item does not exists");

            var addItemResult = player.AddItemToInventory(item, request.Amount);
            if (addItemResult.IsFailure) return addItemResult.ErrorList;

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
