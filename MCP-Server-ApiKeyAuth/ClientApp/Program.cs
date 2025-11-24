using ClientApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using HttpClientTransport = ModelContextProtocol.Client.HttpClientTransport;

var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(LLMConfig.DeploymentOrModelId, 
                                                                  LLMConfig.Endpoint,
                                                                  LLMConfig.ApiKey);

builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

var clientTransport = new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint = new Uri("http://localhost:5080"),
    AdditionalHeaders = new Dictionary<string, string> { { "Authorization", "KEY-Xyz-1234" } }
});

var client = await McpClient.CreateAsync(clientTransport);

var tools = await client.ListToolsAsync();

foreach (var tool in tools)
    Console.WriteLine($"{tool.Name}: {tool.Description}");

Kernel kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

kernel.Plugins.AddFromFunctions("weather", tools.Select(t => t.AsKernelFunction()));

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var chatHistory = new ChatHistory();

string? userInput;
do
{
    Console.WriteLine("User> ");
    userInput = Console.ReadLine();

    chatHistory.AddUserMessage(userInput);

    var result = await chatCompletionService.GetChatMessageContentsAsync(
        chatHistory,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    Console.WriteLine("Assistant >" + result);
    //chatHistory.AddMessage(, result.Content ?? string.Empty);
} while (userInput is not null);