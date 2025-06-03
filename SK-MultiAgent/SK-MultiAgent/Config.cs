using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK_MultiAgent;

public class Config
{
    private static readonly IConfigurationRoot configuration;

    static Config()
    {
        configuration = new ConfigurationBuilder()
            .AddUserSecrets<Config>()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
    }
    public static string DeploymentOrModelId => configuration["LLM:DeploymentOrModelId"] ?? "gpt-4o";
    // Default to gpt-4o-mini-2024-08-06 if not set in appsettings.json or user secrets

    // Use the Endpoint if using Azure OpenAI
    public static string Endpoint { get; set; } = "https://api.openai.com/v1/responses";
    public static string ApiKey => configuration["LLM:ApiKey"] ?? throw new InvalidOperationException("API key not found in user secrets.");
}
