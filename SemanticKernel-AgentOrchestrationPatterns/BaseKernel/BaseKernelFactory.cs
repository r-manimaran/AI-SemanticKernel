using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace BaseKernel
{
    public static class BaseKernelFactory
    {
        public static Kernel AzureOpenAIKernel()
        {
            
            var builder = Kernel.CreateBuilder();

            builder.Services.AddLogging(configure => configure.AddConsole());

            builder.Services.AddLogging(configure => configure.SetMinimumLevel(LogLevel.Information));

            builder.Services.AddAzureOpenAIChatCompletion(AzureConfig.DeploymentOrModelId, AzureConfig.Endpoint, AzureConfig.ApiKey);

            return builder.Build();
        }

        public static Kernel OpenAIKernel()
        {
            var builder = Kernel.CreateBuilder();

            builder.Services.AddLogging(configure => configure.AddConsole());

            builder.Services.AddLogging(configure=> configure.SetMinimumLevel(LogLevel.Information));

            builder.Services.AddOpenAIChatCompletion(OpenAIConfig.DeploymentOrModelId, OpenAIConfig.ApiKey);
                        
            return builder.Build();
        }
    }
}
