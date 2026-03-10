using DatabaseMcpServer;
using DatabaseMcpServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind Settings
builder.Services.Configure<DatabaseToolsSettings>(
    builder.Configuration.GetSection("DatabaseTools"));

// Register Services
builder.Services.AddSingleton<QueryGuardrailService>();
builder.Services.AddSingleton<DatabaseService>();

builder.Services.AddMcpServer()
                 .WithHttpTransport(options =>
                 {
                     options.Stateless = true;  // set true for stateless/serverless deployment
                 })
                .WithToolsFromAssembly();

var app = builder.Build();

app.MapMcp("/mcp");

app.Run("http://localhost:5200");


