using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseKernel;

public class AzureConfig
{
    private static readonly IConfigurationRoot configuration;


    static AzureConfig()
    {

        // Load configuration from appsettings.json and user secrets
        string filePath = Path.GetFullPath("appsettings.json");
        configuration = new ConfigurationBuilder()                    
                    .AddJsonFile(filePath)
                    .AddUserSecrets<AzureConfig>()
                    .Build();
    }

    public static string Endpoint => configuration["AzureOpenAIConfig:Endpoint"] ?? "https://your-endpoint.openai.azure.com/";
    public static string ApiKey => configuration["AzureOpenAIConfig:ApiKey"] ?? "yourApiKey";
    public static string DeploymentOrModelId => configuration["AzureOpenAIConfig:DeploymentOrModelId"] ?? "gpt-35-turbo"; // Default model ID
}
