using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgenticProjectDescGenerator;

public class AppConfig
{
    private static readonly IConfiguration _configuration;
    static AppConfig()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<AppConfig>()
            .Build();
    }

    public static string ModelName => _configuration["LLMConfig:OpenAI:Model"] ?? "gpt-4o-mini";
    public static string ApiKey => _configuration["LLMConfig:OpenAI:ApiKey"] ?? throw new InvalidOperationException("API key is not configured.");
    public static string RootPath => _configuration["RootPath"] ?? throw new InvalidOperationException("Root path is not configured.");
}
