using System.Security.Claims;
using MediatR;
using NexusGameEngine.Application.Features.Player.Commands;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.API.Endpoints;

public record MoveInputDto(float X, float Y, float Z);

public static class WorldEndpoints
{
    public static void MapWorldEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/world").WithTags("World").RequireAuthorization("PlayerPol");

        group.MapPost("/move", async (MoveInputDto request,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken) =>
        {
            string? userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid playerId))
            {
                return Results.Unauthorized();
            }

            var query = new MovePlayerCommand(playerId, request.X, request.Y, request.Z);
            var result = await sender.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

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
