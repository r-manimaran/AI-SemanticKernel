using Microsoft.SemanticKernel;

namespace BaseKernel
{
    public static class BaseKernelFactory
    {
        public static Kernel AzureOpenAIKernel()
        {
            
            var builder = Kernel.CreateBuilder();
            builder.Services.AddAzureOpenAIChatCompletion(AzureConfig.DeploymentOrModelId, AzureConfig.Endpoint, AzureConfig.ApiKey);
            return builder.Build();
        }

        public static Kernel OpenAIKernel()
        {
            var builder = Kernel.CreateBuilder();
            builder.Services.AddOpenAIChatCompletion(OpenAIConfig.DeploymentOrModelId, OpenAIConfig.ApiKey);
            return builder.Build();
        }
    }
}
