using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Domain.Entities.Payloads;
using NexusGameEngine.Domain.Enums;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public class UseItemHandler(IApplicationDbContext appDbContext, TimeProvider timeProvider) : IRequestHandler<UseItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UseItemCommand request, CancellationToken cancellationToken)
    {
        var player = await appDbContext.Players
                    .Include(p => p.InventorySlots)
                        .ThenInclude(slot => slot.Item)
                    .FirstOrDefaultAsync(p => p.Id == request.PlayerId, cancellationToken);
        if (player is null) return Error.NotFound("UseItem.PlayerNotFound", "The player does not exists");

        var invSlot = player.InventorySlots.FirstOrDefault(invS => invS.Id == request.InventorySlotId);
        if (invSlot is null) return Error.NotFound("UseItem.InventorySlotNotFound", "The player does not have that inventory slot");

        switch (invSlot.Item.ItemType)
        {
            case ItemType.Consumable:
                var ConsumableResult = await ConsumableItem(JsonSerializer.Deserialize<ConsumablePayload>(invSlot.Item.ActionPayload), player, invSlot, timeProvider);
                if (ConsumableResult.IsFailure) return ConsumableResult.ErrorList;
                break;
            case ItemType.Weapon:
                var WeaponResult = await WeaponItem(JsonSerializer.Deserialize<WeaponPayload>(invSlot.Item.ActionPayload), player, invSlot);
                if (WeaponResult.IsFailure) return WeaponResult.ErrorList;
                break;
            case ItemType.Armor:
                var ArmorResult = await ArmorItem(JsonSerializer.Deserialize<ArmorPayload>(invSlot.Item.ActionPayload), player, invSlot);
                if (ArmorResult.IsFailure) return ArmorResult.ErrorList;
                break;
            case ItemType.Misc:
                var MiscResult = MiscItem(JsonSerializer.Deserialize<MiscPayload>(invSlot.Item.ActionPayload), player, invSlot);
                if (MiscResult.IsFailure) return MiscResult.ErrorList;
                break;
        }

        await appDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static async Task<Result<bool>> ConsumableItem(ConsumablePayload? consumablePayload, Domain.Entities.Player player, InventorySlot invSlot, TimeProvider timeProvider)
    {
        if (consumablePayload is null) return Error.Validation("UseItem.InvalidPayload", "Cannot read item data");

        if (consumablePayload.HealAmount.HasValue)
        {
            var healSuccess = player.Heal(consumablePayload.HealAmount.Value);
            if (healSuccess.IsFailure) return healSuccess.ErrorList;
        }
        if (consumablePayload.StaminaAmount.HasValue)
        {
            var gStaminaResult = player.GainStamina((short)consumablePayload.StaminaAmount.Value, timeProvider.GetUtcNow());
            if (gStaminaResult.IsFailure) return gStaminaResult.ErrorList;
        }

        var removeResult = invSlot.RemoveQuantity(1);
        if (removeResult.IsFailure) return removeResult.ErrorList;

        if (invSlot.Quantity == 0)
        {
            var removeInvSlotResult = player.RemoveInventorySlot(invSlot);
            if (removeInvSlotResult.IsFailure) return removeInvSlotResult.ErrorList;
        }

        return true;
    }

    private static Task<Result<bool>> WeaponItem(WeaponPayload? weaponPayload, Domain.Entities.Player player, InventorySlot invSlot)
    {
        // TODO: Questa logica va implementata successivamente.
        // Voglio creare un'Epic a parte per implementare e gestire l'intero sistema di Equipaggiamento.
        return Task.FromResult<Result<bool>>(Error.Validation("UseItem.CannotConsume", "Le armi non possono essere consumate in questo modo. Usa l'azione corretta per equipaggiarle."));
    }


    private static Task<Result<bool>> ArmorItem(ArmorPayload? armorPayload, Domain.Entities.Player player, InventorySlot invSlot)
    {
        // TODO: Questa logica va implementata successivamente.
        // Voglio creare un'Epic a parte per implementare e gestire l'intero sistema di Equipaggiamento.
        return Task.FromResult<Result<bool>>(Error.Validation("UseItem.CannotConsume", "Le armature non possono essere consumate in questo modo. Usa l'azione corretta per equipaggiarle."));
    }
    private static Result<bool> MiscItem(MiscPayload? miscPayload, Domain.Entities.Player player, InventorySlot invSlot)
    {
        return Error.Validation("UseItem.NotUsable", "This item cannot be used. Can be sold, exchanged or used in a crafting recipe");
    }

}
