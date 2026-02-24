
using AgenticProjectDescGenerator;
using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// 1. Setup the kernel and register the plugin
var builder = Kernel.CreateBuilder();
builder.Services.AddLogging(services=> services.AddConsole().SetMinimumLevel(LogLevel.Information));

builder.Services.AddSingleton<IFunctionInvocationFilter, PluginLoggingFilter>();

builder.AddOpenAIChatCompletion(AppConfig.ModelName, AppConfig.ApiKey);

builder.Plugins.AddFromType<ProjectFilePlugin>();
builder.Plugins.AddFromType<ProjectContextPlugin>();

var kernel = builder.Build();

// 2. Define the Agent
ChatCompletionAgent projectAgent = new()
{
    Name = "ProjectAnalyst",
    Instructions = @"Youare a technical architect. Your goal is to:
            1. Find all .NET project folders in the specified root path.
            2. Read the content of the .csproj file and ReadMe.md file in each project folder.
            3. Extract the Title, Description and Categorized Tech Stack.
            4. Summarize the findings in a concise JSON array.",
    Kernel = kernel,
    Arguments = new KernelArguments(
        new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        })
};

ChatCompletionAgent architectAgent = new()
{
    Name = "ProjectArchitect",
    Instructions = @"You are a Senior Systems Architect. I will provide you with 'Context Signals' from a folder.
    
    Your task is to triangulate these signals to describe the project:
    1. If there is a README, prioritize it.
    2. If there are 'Controllers', it is likely a Web API or MVC app. 
    3. Look at the 'Program.cs' to see what services are registered (e.g., AddDbContext, AddAuthentication).
    4. If the folder names contain 'Tests', categorize it as a Testing Suite.

    Based on these dots, 'hallucinate' a professional description that explains the *intent* of the code, not just the files present.",
    Kernel = kernel
};

// 3. Run the Agent
var history = new ChatHistory();
string rootPath = AppConfig.RootPath;
string userMessage = $"Scan the projects in {rootPath} and provide a summary of each project in JSON format.";
history.AddUserMessage(userMessage);

//await foreach( var message in projectAgent.InvokeAsync(history))    
//{
//    AgentResponseItem<ChatMessageContent> agentMessage = message;
//    Console.WriteLine(agentMessage.Message);
//}
//Console.ReadLine();

await foreach( var message in architectAgent.InvokeAsync(history))    
{
    AgentResponseItem<ChatMessageContent> agentMessage = message;
    Console.WriteLine(agentMessage.Message);
}



public record ProjectSummary(string Title, string Description, List<string> Tech);