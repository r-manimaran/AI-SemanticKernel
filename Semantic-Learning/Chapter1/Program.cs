using Chapter1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// Read Configuration and based on config call the below methods
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

string llm = config["llm"] ?? "OpenAI";

switch (llm)
{
    case "OpenAI":
        CallOpenAIChat().GetAwaiter().GetResult();
        break;
    case "AzureOpenAI":
        CallAzureOpenAIChat().GetAwaiter().GetResult();
        break;
    default:
        Console.WriteLine("Invalid LLM");
        break;
}

static async Task CallOpenAIChat()
{

    //create the logger factory
    using var loggerFactory = LoggerFactory.Create(builder=> builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

    // create a kernel with OpenAI
    var builder = Kernel.CreateBuilder()
                    .AddOpenAIChatCompletion(OpenAIConfig.DeploymentOrModelId, OpenAIConfig.ApiKey);
                    //.Services.AddSingleton(loggerFactory);


    Kernel kernel = builder.Build();

    var logger = loggerFactory.CreateLogger("Semantic Kernel");
    logger.LogInformation("Starting OpenAI chat completion");

    // Test the chat completion
    var result = await kernel.InvokePromptAsync("Create a sample FastApi project?");
    logger.LogInformation("Chat completion successful");
    Console.WriteLine(result);
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}

static async Task CallAzureOpenAIChat()
{
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole
                            ().SetMinimumLevel(LogLevel.Debug));

    // create a kernel with Azure OpenAI
    var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(AzureOpenAIConfig.DeploymentOrModelId, AzureOpenAIConfig.Endpoint, AzureOpenAIConfig.ApiKey);
    Kernel kernel = builder.Build();

    var logger = loggerFactory.CreateLogger("Semantic Kernel");
    logger.LogInformation("Starting Azure OpenAI chat completion");
    // Test the chat completion
    var result = await kernel.InvokePromptAsync("Create a sample FastApi project?");
    Console.WriteLine(result);
    logger.LogInformation("Chat completion successful");
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}