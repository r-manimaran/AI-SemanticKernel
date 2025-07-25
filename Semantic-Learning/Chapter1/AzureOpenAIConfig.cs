using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter1;

public class AzureOpenAIConfig
{
    private static readonly IConfigurationRoot configuration;
  

    static AzureOpenAIConfig()
    {     

        // Load configuration from appsettings.json and user secrets
        string filePath = Path.GetFullPath("appsettings.json");
        configuration = new ConfigurationBuilder()
            .AddUserSecrets<AzureOpenAIConfig>()
            .AddJsonFile(filePath)            
            .Build();
    }

    public static string Endpoint => configuration["AzureOpenAI:Endpoint"] ?? "https://your-endpoint.openai.azure.com/";
    public static string ApiKey => configuration["AzureOpenAI:ApiKey"] ?? "yourApiKey";
    public static string DeploymentOrModelId => configuration["AzureOpenAI:DeploymentNameModelId"] ?? "gpt-35-turbo"; // Default model ID
}
