using IMS.Application;
using IMS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .RegisterSQLServerPersistenceFromInfrastructure()
    .RegisterCommandsAndQueriesFromApplication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // TODO: Fill initial data in this case
}

// Update database in container
app.ApplySQLServerMigrationsFromInfrastructure();

app.MapGet("/health", () =>
{
    
    return TypedResults.Ok(new { status = "Healthy" });
})
.WithName("GetHealthStatus")
.WithOpenApi();

app.Run();