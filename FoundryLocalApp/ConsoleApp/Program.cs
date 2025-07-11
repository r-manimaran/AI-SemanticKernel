
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

string modelId = "qwen2.5-1.5b-instruct-generic-gpu";
Uri endpointFoundryLocal = new Uri("http://localhost:5273/v1");

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddOpenAIChatCompletion(modelId, endpointFoundryLocal, "*");

var kernel = kernelBuilder.Build();
OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings
{
    ChatSystemPrompt = "You are a expert in .net C# developement.",
    Temperature = 0.7f,
    MaxTokens = 32768,
};

var response = kernel.InvokePromptStreamingAsync("Write a .Net C# code that demonstrate the simple binary search", new(settings));

await foreach (var chunk in response.ConfigureAwait(false))
{
    Console.Write(chunk);
}