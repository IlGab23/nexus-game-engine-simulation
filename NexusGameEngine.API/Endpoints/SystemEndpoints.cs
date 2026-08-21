namespace NexusGameEngine.API.Endpoints;

public static class SystemEndpoints
{
    public static void MapSystemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/system")
                        .WithTags("System");

        group.MapGet("/ping", async () => Results.Ok("PONG!"));

        group.MapGet("/admin-ping", async () => Results.Ok("ADMIN PONG!"))
             .RequireAuthorization("AdminPol");

    }
}
