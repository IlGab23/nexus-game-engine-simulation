using NexusGameEngine.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring native OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

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

app.MapSystemEndpoints();

app.Run();
