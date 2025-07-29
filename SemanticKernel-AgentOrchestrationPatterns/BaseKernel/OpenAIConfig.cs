using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseKernel;

public class OpenAIConfig
{
    private static readonly IConfigurationRoot configuration;
    static OpenAIConfig()
    {
        configuration = new ConfigurationBuilder()             
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .AddUserSecrets<OpenAIConfig>()
             .Build();

    }

    public static string DeploymentOrModelId => configuration["OpenAIConfig:DeploymentOrModelId"] ?? "gpt-35-turbo";
    public static string ApiKey => configuration["OpenAIConfig:ApiKey"] ?? "yourApiKey";
}
