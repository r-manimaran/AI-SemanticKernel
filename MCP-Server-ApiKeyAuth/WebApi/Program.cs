using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseMiddleware<AuthMiddleware>();

app.MapMcp();

app.Run();

