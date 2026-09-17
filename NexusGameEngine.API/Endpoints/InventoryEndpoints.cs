using System.Security.Claims;
using MediatR;
using NexusGameEngine.Application.Features.Player.Commands;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.API.Endpoints;

public record InventoryUseItemInputDTO(Guid InventorySlotId);

public static class InventoryEndpoints
{
    public static void MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/player/inv").WithTags("PlayerInv").RequireAuthorization("PlayerPol");

        group.MapPost("/use-item", async (InventoryUseItemInputDTO request, HttpContext context, ISender sender, CancellationToken cancellationToken) =>
        {
            string? userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid playerId))
            {
                return Results.Unauthorized();
            }

            var command = new UseItemCommand(playerId, request.InventorySlotId);
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
