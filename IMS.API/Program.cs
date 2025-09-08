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
    // TODO: Fill initial data in this case
}

// Update database in container
app.ApplySQLServerMigrationsFromInfrastructure();
app.MapEndpointsFromPresentation();

app.Run();