using System.Security.Claims;
using MediatR;
using NexusGameEngine.Application.Features.Player.Queries;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.API.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/players").WithTags("Players").RequireAuthorization("PlayerPol");

        group.MapGet("/me", async (HttpContext context,
        ISender sender,
        CancellationToken cancellationToken) =>
        {
            string? userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid playerId))
            {
                return Results.Unauthorized();
            }

            var query = new GetPlayerProfileQuery(playerId);
            var result = await sender.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            var errorType = result.ErrorList.FirstOrDefault()?.Type;

            return errorType switch
            {
                ErrorType.NotFound => Results.NotFound(result.ErrorList),
                _ => Results.BadRequest(result.ErrorList)
            };
        });
    }
}
