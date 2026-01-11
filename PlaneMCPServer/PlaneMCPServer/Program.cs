using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlaneMCPServer;

var builder = Host.CreateApplicationBuilder(args);

// Prevent the host from writing logs to stdout which would corrupt the MCP stdio JSON protocol
builder.Logging.ClearProviders();

builder.Configuration.Sources.Clear();
builder.Configuration.SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true);
    //.AddEnvironmentVariables();

var planeApiKey = builder.Configuration["PlaneAPIKey"];
var baseUrl = builder.Configuration["BaseUrl"];
var workspace = builder.Configuration["Workspace"];
var projectId = builder.Configuration["ProjectId"];

if (string.IsNullOrEmpty(planeApiKey) ||
    string.IsNullOrEmpty(baseUrl) ||
    string.IsNullOrEmpty(workspace) ||
    string.IsNullOrEmpty(projectId))
{
    // Write to stderr and exit to avoid any output on stdout that could break MCP protocol
    Console.Error.WriteLine("Missing required configuration values. Ensure PlaneAPIKey, BaseUrl, Workspace and ProjectId are set in appsettings or environment.");
    Environment.Exit(1);
}

builder.Services.AddHttpClient();
builder.Services.AddSingleton<PlaneService>(sp=>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return new PlaneService(
        httpClientFactory,
        sp.GetRequiredService<ILogger<PlaneService>>(),
        baseUrl,
        workspace,
        projectId,
        planeApiKey);
    
});
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

await app.RunAsync();
