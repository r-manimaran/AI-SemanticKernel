using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter1;

public class OpenAIConfig
{
    private static readonly IConfigurationRoot configuration;
  
    static OpenAIConfig()
    {
        configuration = new ConfigurationBuilder()
             .AddUserSecrets<OpenAIConfig>()
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .Build(); 
       
     }

    public static string DeploymentOrModelId => configuration["LLM:DeploymentOrModelId"] ?? "gpt-35-turbo";
    public static string ApiKey => configuration["LLM:ApiKey"] ?? "yourApiKey";
}
