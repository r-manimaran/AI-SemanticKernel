using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using PluginsApp;
using PluginsApp.Plugins.Lights;
using PluginsApp.Plugins.Weather;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// To Use OpenAI Chat Completion with Plugins
var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(Config.DeploymentOrModelId, Config.ApiKey);

// To use Azure OpenAI Chat Completion with Plugins
//var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(AzureOpenAIConfig.ModelName, AzureOpenAIConfig.Endpoint, AzureOpenAIConfig.ApiKey);

builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

Console.Clear();

// Add the weather pulgin
builder.Services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
    .AddUserSecrets<Config>()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build());

builder.Plugins.AddFromType<WeatherPlugin>("Weather");

// Add the lights plugin
builder.Plugins.AddFromType<LightsPlugin>("Lights");

// Add Individual Plugins
builder.Plugins.AddFromFunctions("time_plugin", [
    KernelFunctionFactory.CreateFromMethod(
        method: () => DateTime.Now.ToString("F"),
        functionName:"get_time",
        description: "Get the current time in a human-readable format."),
    KernelFunctionFactory.CreateFromMethod(
            method:(DateTime start, DateTime end) => (end-start).TotalSeconds,
            functionName: "diff_time",
            description: "Calculate the difference in seconds between two DateTime objects."
            )
    ]);

Kernel kernel = builder.Build();

Console.WriteLine("Welcome to the Sematic Kernel Plugins App with Weather and Light Plugins.");
Console.WriteLine("---------------------------------------------------------------------------");
Console.WriteLine(" ");

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Enable planning and execution
OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings
{
 FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};


// create history 
var history = new ChatHistory();
string? userInput;
do
{
    // collect user input
    Console.Write("User > ");
    userInput = Console.ReadLine(); //"Please turn on the Lamp"
    history.AddUserMessage(userInput);


    // Get the Response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: settings,
        kernel: kernel);

    Console.WriteLine("Assistant >" + result);

    history.AddAssistantMessage(result.Content?? string.Empty);
    //history.AddMessage(result.Role, result.Content?? string.Empty);
} 
while(userInput is not null && userInput.ToLower() != "exit");

foreach (var h in history)
{
    Console.WriteLine($"{h.Role} > {h.Content} ");
}

Console.ReadLine();