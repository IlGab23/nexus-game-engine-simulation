using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Domain.Entities.ValueObjects;

using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public class MovePlayerHandler(IApplicationDbContext appDbContext, TimeProvider timeProvider) : IRequestHandler<MovePlayerCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MovePlayerCommand request, CancellationToken cancellationToken)
    {
        var player = await appDbContext.Players.FirstOrDefaultAsync(p => p.Id == request.PlayerId, cancellationToken);
        if (player is null) return Error.NotFound("MovePlayerHandler.PlayerNotFound", "Player does not exists");

        var nowTime = timeProvider.GetUtcNow();

        var newPosResult = Vector3.Create(request.TargetX, request.TargetY, request.TargetZ);
        if (newPosResult.IsFailure) return newPosResult.ErrorList;

        var playerMovedResult = player.MoveTo(newPosResult.Value, nowTime);
        if (playerMovedResult.IsFailure) return playerMovedResult.ErrorList;

        await appDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

}
