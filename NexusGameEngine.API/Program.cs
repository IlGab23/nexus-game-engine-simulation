using System.Security.Claims;
using System.Threading.RateLimiting;
using NexusGameEngine.API.Endpoints;
using NexusGameEngine.Application;
using NexusGameEngine.Infrastructure;
using NexusGameEngine.Infrastructure.Exceptions;
using NexusGameEngine.Infrastructure.Persistance.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? throw new InvalidOperationException("Cors.MissingOrgins: Missing origins link in configuration file");

builder.Services.AddCors(opz =>
{
    opz.AddPolicy("FrontendPolicy", pol =>
    {
        pol.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(opz =>
{
    opz.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    opz.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userId))
        {
            int loggedTimeLimit = builder.Configuration.GetValue<int?>("RateLimiting:LoggedUser:TimeLimitInSeconds") ?? throw new InvalidOperationException("RateLimit.MissingLoggedUserTimeLimitInSeconds: Missing in configuration file");
            return RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: $"user_{userId}",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = builder.Configuration.GetValue<int?>("RateLimiting:LoggedUser:PermitLimit") ?? throw new InvalidOperationException("RateLimit.MissingLoggedUserPermitLimit: Missing in configuration file"),
                    Window = TimeSpan.FromSeconds(loggedTimeLimit),
                    SegmentsPerWindow = builder.Configuration.GetValue<int?>("RateLimiting:LoggedUser:SegmentsPerWindow") ?? throw new InvalidOperationException("RateLimit.MissingLoggedUserSegmentsPerWindow: Missing in configuration file")
                }

            );
        }

        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";
        int ipTimeLimit = builder.Configuration.GetValue<int?>("RateLimiting:IpUser:TimeLimitInSeconds") ?? throw new InvalidOperationException("RateLimit.MissingIpUserTimeLimitInSeconds: Missing in configuration file");
        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: $"ip_{clientIp}",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = builder.Configuration.GetValue<int?>("RateLimiting:IpUser:PermitLimit") ?? throw new InvalidOperationException("RateLimit.MissingIpUserPermitLimit: Missing in configuration file"),
                Window = TimeSpan.FromSeconds(ipTimeLimit),
                SegmentsPerWindow = builder.Configuration.GetValue<int?>("RateLimiting:IpUser:SegmentsPerWindow") ?? throw new InvalidOperationException("RateLimit.MissingIpUserSegmentsPerWindow: Missing in configuration file")
            }

        );
    });
});

// Learn more about configuring native OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();

    await seeder.InitializeDatabaseAsync(CancellationToken.None);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Generazione OpenAPI nativa di .NET

    app.UseSwagger(); // Generazione documento Swagger (Swashbuckle)
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nexus Game Engine API v1");
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthentication();

app.UseRateLimiter();

app.UseAuthorization();

app.MapSystemEndpoints();

app.Run();
