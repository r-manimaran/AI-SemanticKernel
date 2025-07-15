using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginsApp;

public class AzureOpenAIConfig
{
    private static readonly IConfigurationRoot configuration;
    static AzureOpenAIConfig()
    {
        configuration = new ConfigurationBuilder()
           .AddUserSecrets<Config>()
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .Build();
    }
    public static string ModelName => configuration["AzureOpenAI:DeploymentOrModelId"] ?? throw new InvalidOperationException("Model name not found in user secrets.");
    public static string Endpoint  => configuration["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException("Endpoint not found in user secrets.");
    public static string ApiKey => configuration["AzureOpenAI:ApiKey"] ?? throw new InvalidOperationException("API key not found in user secrets.");

}
