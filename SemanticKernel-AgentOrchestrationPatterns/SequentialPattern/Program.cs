
using BaseKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using SequentialPattern;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<AgentService>();
            }).Build();

        var logger = host.Services.GetRequiredService<ILogger<AgentService>>();
        
        var agentService = host.Services.GetRequiredService<AgentService>();

        await agentService.RunAsync(logger);             

        Console.Read();
    }     

}