using MediatR;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public record ClaimDailyRewardCommand(Guid PlayerId) : IRequest<Result<DailyRewardResponse>>;

public record DailyRewardResponse(int? MoneyGained, string? ItemNameGained, int? ItemAmountGained);
