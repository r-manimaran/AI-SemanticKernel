using LunchTimeMcpApp.MCPTools;
using LunchTimeMcpApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RestaurantTools>();

builder.Services.AddScoped<IRestaurantService, RestaurantService>();

await builder.Build().RunAsync();
