using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Application.Interfaces.Security;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Domain.ResultPattern;
using DomainUser = NexusGameEngine.Domain.Entities.User;

namespace NexusGameEngine.Application.Features.RefreshTokenFeat.Commands;

public class LoginWithRefreshTokenHandler(IApplicationDbContext appDbContext, IJwtTokenProvider jwtTokenProvider, IRefreshTokenProvider refreshTokenProvider, TimeProvider timeProvider) : IRequestHandler<LoginWithRefreshTokenCommand, Result<LoginWithRefreshTokenCommandOutput>>
{
    public async Task<Result<LoginWithRefreshTokenCommandOutput>> Handle(LoginWithRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var nowTime = timeProvider.GetUtcNow();
        string oldTokenHash = refreshTokenProvider.ConvertToString(refreshTokenProvider.CalculateHash(request.OldRefreshToken));
        RefreshToken? oldDbToken = await appDbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == oldTokenHash, cancellationToken);
        if (oldDbToken is null) return Error.NotFound("RefreshToken.NotFound", "Request refresh token not found");
        DomainUser user = await appDbContext.Users
                                .AsNoTracking()
                                .Include(u => u.SystemRole)
                                .FirstAsync(u => u.Id == oldDbToken.UserId, cancellationToken);

        if (!oldDbToken.IsActive(timeProvider))
        {
            if (oldDbToken.IsRevoked)
            {
                await appDbContext.RefreshTokens
                .Where(rt => rt.UserId == oldDbToken.UserId && rt.RevokeAt == null)
                .ExecuteUpdateAsync(s => s
                .SetProperty(rt => rt.RevokeAt, nowTime)
                .SetProperty(rt => rt.ReplacedByTokenHash, "COMPROMISED"),
                cancellationToken);

                return Error.RefreshTokenCompromised;
            }
            return Error.RefreshTokenExpired;
        }

        string newJwt = jwtTokenProvider.GenerateJWT(user);

        string newRefreshToken = refreshTokenProvider.GenerateRT();
        string newRefreshTokenHash = refreshTokenProvider.ConvertToString(refreshTokenProvider.CalculateHash(newRefreshToken));
        var newRefreshTokenResult = RefreshToken.Create(user.Id, newRefreshTokenHash, refreshTokenProvider.GetExpiryTime(timeProvider));
        await appDbContext.RefreshTokens.AddAsync(newRefreshTokenResult.Value, cancellationToken);
        oldDbToken.Revoke(nowTime, newRefreshToken);

        await appDbContext.SaveChangesAsync(cancellationToken);

        return new LoginWithRefreshTokenCommandOutput(newJwt, newRefreshToken);
    }

}
