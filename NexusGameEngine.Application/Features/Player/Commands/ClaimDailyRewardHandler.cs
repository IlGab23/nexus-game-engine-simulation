using MediatR;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public class ClaimDailyRewardHandler(IApplicationDbContext appDbContext, TimeProvider timeProvider) : IRequestHandler<ClaimDailyRewardCommand, Result<DailyRewardResponse>>
{
    public Task<Result<DailyRewardResponse>> Handle(ClaimDailyRewardCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

}
