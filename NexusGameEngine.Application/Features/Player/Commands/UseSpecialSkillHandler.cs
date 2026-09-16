using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Domain.Constants;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public class UseSpecialSkillHandler(IApplicationDbContext appDbContext, TimeProvider timeProvider) : IRequestHandler<UseSpecialSkillCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UseSpecialSkillCommand request, CancellationToken cancellationToken)
    {
        var player = await appDbContext.Players
                            .Include(p => p.Cooldowns)
                            .FirstOrDefaultAsync(p => p.Id == request.PlayerId, cancellationToken);
        if (player is null) return Error.NotFound("Player.NotFound", "The player does not exist");

        if (!player.ActiveSpecialSkill.HasValue) return Error.Conflict("Player.NoSpecialSkill", "The player doesn't have a special skill equipped");

        var nowTime = timeProvider.GetUtcNow();
        var newSkillDelay = nowTime + GameConstants.ActionCooldowns.UseSpecialSkillCooldown;

        var tryStartCooldownResult = player.TryStartCooldown(GameConstants.Actions.UseSpecialSkill, newSkillDelay, nowTime);
        if (tryStartCooldownResult.IsFailure) return tryStartCooldownResult.ErrorList;

        await appDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

}
