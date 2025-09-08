using IMS.Application;
using IMS.Infrastructure;
using IMS.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .RegisterSQLServerPersistenceFromInfrastructure()
    .RegisterCommandsAndQueriesFromApplication()
    .RegisterEndpointsFromPresentation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Update database in container with optional seed
await app.ApplySQLServerMigrationsFromInfrastructure(seed: true);
app.MapEndpointsFromPresentation();

app.Run();