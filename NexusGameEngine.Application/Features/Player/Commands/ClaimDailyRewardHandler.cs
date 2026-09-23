using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Domain.Constants;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public class ClaimDailyRewardHandler(IApplicationDbContext appDbContext, TimeProvider timeProvider, IRandomProvider rndProvider) : IRequestHandler<ClaimDailyRewardCommand, Result<DailyRewardResponse>>
{
    public async Task<Result<DailyRewardResponse>> Handle(ClaimDailyRewardCommand request, CancellationToken cancellationToken)
    {
        var player = await appDbContext.Players.Include(p => p.InventorySlots)
                                                .FirstOrDefaultAsync(p => p.Id == request.PlayerId, cancellationToken);

        if (player is null) return Error.NotFound("Player.NotFound", "The player does not exist");

        var nowTime = timeProvider.GetUtcNow();
        var nextMidnight = new DateTimeOffset(nowTime.Year, nowTime.Month, nowTime.Day, 0, 0, 0, nowTime.Offset);
        nextMidnight = nextMidnight.AddDays(1);

        var tryStartCooldownResult = player.TryStartCooldown(GameConstants.Actions.ClaimDailyReward, nextMidnight, nowTime);
        if (tryStartCooldownResult.IsFailure) return tryStartCooldownResult.ErrorList;

        int randomChoice = rndProvider.GetRandomNumberInRange(0, 100);

        if (randomChoice <= 49)
        {
            var getMoneyResult = GetMoney(player);
            if (getMoneyResult.IsFailure) return getMoneyResult.ErrorList;

            await appDbContext.SaveChangesAsync(cancellationToken);
            return getMoneyResult.Value;
        }
        else
        {
            var getItemResult = await GetItem(player, appDbContext, cancellationToken);
            if (getItemResult.IsFailure)
            {
                var getMoneyResult = GetMoney(player);
                if (getMoneyResult.IsFailure) return getMoneyResult.ErrorList;

                await appDbContext.SaveChangesAsync(cancellationToken);
                return getMoneyResult.Value;
            }

            await appDbContext.SaveChangesAsync(cancellationToken);
            return getItemResult.Value;
        }

    }

    private Result<DailyRewardResponse> GetMoney(Domain.Entities.Player player)
    {
        int choicedMoneyValue = System.Random.Shared.GetItems(GameConstants.DailyRewardData.RandomMoney, 1)[0];

        var addMoneyResult = player.AddMoney(choicedMoneyValue);
        if (addMoneyResult.IsFailure) return addMoneyResult.ErrorList;

        return new DailyRewardResponse(choicedMoneyValue, null, null);
    }

    private async Task<Result<DailyRewardResponse>> GetItem(Domain.Entities.Player player, IApplicationDbContext appDbContext, CancellationToken cancellationToken)
    {
        var item = await appDbContext.Items.OrderBy(i => EF.Functions.Random()).FirstOrDefaultAsync(cancellationToken);
        if (item is null) return Error.NotFound("DailyRewardItem.NotFound", "An error occured trying to get the daily reward item");
        int amount = item.MaxStackQuantity > 1 ? 3 : 1;

        var addItemResult = player.AddItemToInventory(item, amount);
        if (addItemResult.IsFailure) return addItemResult.ErrorList;

        return new DailyRewardResponse(null, item.Name, amount);
    }

}
