using System.Security.Claims;
using MediatR;
using NexusGameEngine.Application.Features.Player.Commands;
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

            return ReturnErrorRequest(result.ErrorList);
        });

        group.MapPost("/UseSpecialSkill", async (HttpContext context,
        ISender sender,
        CancellationToken cancellationToken) =>
        {
            string? userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid playerId)) return Results.Unauthorized();

            var command = new UseSpecialSkillCommand(playerId);
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess) return Results.Ok(result.Value);

            return ReturnErrorRequest(result.ErrorList);
        });

        group.MapPost("/ClaimDailyReward", async (HttpContext context,
        ISender sender,
        CancellationToken cancellationToken) =>
        {
            string? userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid playerId)) return Results.Unauthorized();

            var command = new ClaimDailyRewardCommand(playerId);
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess) return Results.Ok(result.Value);

            return ReturnErrorRequest(result.ErrorList);
        });


    }

    private static IResult ReturnErrorRequest(List<Error> errors)
    {
        ErrorType? errorType = errors.FirstOrDefault()?.Type;
        return errorType switch
        {
            ErrorType.NotFound => Results.NotFound(errors),
            ErrorType.Conflict => Results.Conflict(errors),
            ErrorType.Failure => Results.InternalServerError(errors),
            _ => Results.BadRequest(errors)
        };
    }
}
