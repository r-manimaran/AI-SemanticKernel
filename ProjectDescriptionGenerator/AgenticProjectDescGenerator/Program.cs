
using AgenticProjectDescGenerator;
using Microsoft.Agents.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// 1. Setup the kernel and register the plugin
var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(AppConfig.ModelName, AppConfig.ApiKey);

builder.Plugins.AddFromType<ProjectFilePlugin>();
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

// 3. Run the Agent
var history = new ChatHistory();
string rootPath = AppConfig.RootPath;
string userMessage = $"Scan the projects in {rootPath} and provide a summary of each project in JSON format.";
history.AddUserMessage(userMessage);

await foreach( var message in projectAgent.InvokeAsync(history))    
{
    AgentResponseItem<ChatMessageContent> agentMessage = message;
    Console.WriteLine(agentMessage.Message);
}
Console.ReadLine();