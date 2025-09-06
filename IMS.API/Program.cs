var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/health", () =>
{
    return TypedResults.Ok(new { status = "Healthy" });
})
.WithName("GetHealthStatus")
//.WithOpenApi()
;

app.Run();
