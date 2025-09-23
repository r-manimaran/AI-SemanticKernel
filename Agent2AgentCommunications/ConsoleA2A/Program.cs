using A2A;
using A2A.AspNetCore;
using ConsoleA2A.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.A2A;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient().AddLogging();

builder.Configuration.AddUserSecrets<Program>();

var app= builder.Build();

List<KernelPlugin> plugins = [KernelPluginFactory.CreateFromType<PolicyInfoTools>()];

string modelId = "llama3.2:3b";

Uri endpoint = new Uri("http://localhost:11434");

var kernelBuilder = Kernel.CreateBuilder();
// using Ollama
//kernelBuilder.Services.AddOllamaChatCompletion(modelId, endpoint); // commented as the ollama running in docker getting more time to get response.

// using OpenAI
string openAIKey = builder.Configuration["OPENAI_API_KEY"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
kernelBuilder.Services.AddOpenAIChatCompletion("gpt-4o", openAIKey);

plugins.ForEach(plugin => kernelBuilder.Plugins.Add(plugin));

var kernel = kernelBuilder.Build();

PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

ChatCompletionAgent agent = new()
{
    Name = "PolicyAdministrator",
    Instructions = "You are a policy admin in an insurance company. Return information on insurance policy only",
    Kernel = kernel,
    Arguments = new(settings)
};
AgentCard agentCard = GetAgentCard();

A2AHostAgent hostAgent = new(agent, agentCard);

app.MapA2A(hostAgent.TaskManager, "/");

await app.RunAsync();

static AgentCard GetAgentCard()
{
    AgentCapabilities capabilities = new AgentCapabilities()
    {
        Streaming = false,
        PushNotifications = true
    };

    AgentSkill skill = new()
    {
        Id = "AgentOne",
        Name = "InsurancePolicyAgent",
        Description = "Handles requests related to insurance policies and customer queries",
        Tags = ["insurance policy"],
        Examples = ["Provide details for policy PA90090"]
    };

    return new AgentCard()
    {
        Name = "PolicyAgent",
        Description = "Handles requests to view insurance policy details",
        Version = "1.0.0",
        DefaultInputModes = ["text"],
        DefaultOutputModes = ["text"],
        Capabilities = capabilities,
        Skills = [skill]
    };
}

