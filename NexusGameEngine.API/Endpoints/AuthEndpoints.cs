
using MediatR;
using NexusGameEngine.Application.Features.RefreshTokenFeat.Commands;
using NexusGameEngine.Application.Features.User.Commands;
using NexusGameEngine.Application.Interfaces.Security;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (HttpContext context,
        LoginUserCommand command,
        ISender sender,
        CancellationToken cancellationToken,
        TimeProvider timeProvider,
        IRefreshTokenProvider refreshTokenProvider) =>
        {
            var result = await sender.Send(command, cancellationToken);

            if (result.IsFailure) return Results.Unauthorized();

            var jwt = result.Value.JwtToken;
            var rt = result.Value.RefreshToken;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenProvider.GetExpiryTime(timeProvider)
            };

            context.Response.Cookies.Append("X-Refresh-Token", rt, cookieOptions);

            return Results.Ok(jwt);
        });

        group.MapPost("/rt-login", async (
            HttpContext context,
            ISender sender,
            CancellationToken cancellationToken,
            TimeProvider timeProvider,
            IRefreshTokenProvider refreshTokenProvider) =>
        {
            if (!context.Request.Cookies.TryGetValue("X-Refresh-Token", out var OldRefreshToken)) return Results.Unauthorized();
            var result = await sender.Send(new LoginWithRefreshTokenCommand(OldRefreshToken), cancellationToken);

            if (result.IsFailure) return Results.Unauthorized();

            var jwt = result.Value.NewJTW;
            var rt = result.Value.NewRefreshToken;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenProvider.GetExpiryTime(timeProvider)
            };

            context.Response.Cookies.Append("X-Refresh-Token", rt, cookieOptions);

            return Results.Ok(jwt);
        });
    }
}
