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

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.MapSystemEndpoints();

app.Run();
