using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HandoffOrchestrationApp;

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

        while (true)
        {
            Console.WriteLine("Enter the Query input here. For exit, type exit or close:");

            string task = Console.ReadLine();

            if (task.ToLower() == "exit" || task.ToLower() == "close")
                break;
            
            await agentService.RunAsync(logger, task);            
        }

        Console.ReadLine();
    }
}
