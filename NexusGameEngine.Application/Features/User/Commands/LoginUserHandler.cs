using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Application.Interfaces.Security;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Domain.ResultPattern;
using DomainUser = NexusGameEngine.Domain.Entities.User;

namespace NexusGameEngine.Application.Features.User.Commands;

public class LoginUserHandler(IApplicationDbContext appDbContext, IJwtTokenProvider jwtTokenProvider, IPasswordHasher passwordHasher, IRefreshTokenProvider refreshTokenProvider, TimeProvider timeProvider) : IRequestHandler<LoginUserCommand, Result<LoginCommandOutput>>
{
    public async Task<Result<LoginCommandOutput>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        DomainUser? user = await appDbContext.Users.AsNoTracking()
                            .Include(u => u.SystemRole)
                            .FirstOrDefaultAsync(u => u.Email.Value == request.Email, cancellationToken);

        if (user is null) return Error.LoginWrongCredentials;

        if (!await passwordHasher.VerifyPassword(request.Password, user.PasswordHash)) return Error.LoginWrongCredentials;

        string jwToken = jwtTokenProvider.GenerateJWT(user);

        string rToken = refreshTokenProvider.GenerateRT();
        byte[] rTokenHashBytes = refreshTokenProvider.CalculateHash(rToken);
        string rTokenHash = refreshTokenProvider.ConvertToString(rTokenHashBytes);
        var RefreshTokenResult = RefreshToken.Create(user.Id, rTokenHash, refreshTokenProvider.GetExpiryTime(timeProvider));
        await appDbContext.RefreshTokens.AddAsync(RefreshTokenResult.Value, cancellationToken);

        await appDbContext.SaveChangesAsync(cancellationToken);

        return new LoginCommandOutput(jwToken, rToken);
    }

}
