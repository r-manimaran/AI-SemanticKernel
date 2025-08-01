using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConcurrentPattern;

internal class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
        {
            services.AddSingleton<AgentService>();
        }).Build();

        var logger = host.Services.GetRequiredService<ILogger<AgentService>>();

        var agentService = host.Services.GetRequiredService<AgentService>();

        await agentService.RunAsync(logger);

        Console.ReadLine();
    }
}
