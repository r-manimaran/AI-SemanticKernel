using Azure.AI.Inference;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Client;
using System.ClientModel;
using System.Text;

var builder = Host.CreateApplicationBuilder(args);
var config = builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();
var deploymentName = config["AzureOpenAI:DeploymentName"] ?? "gpt-4.1-mini";

var hfHeaders = new Dictionary<string, string>
{
    {"Authorization", $"Bearer {config["HF_TOKEN"]}" }
};
var clientTransport = new SseClientTransport(
    new()
    {
        Name = "HuggingFace",
        Endpoint = new Uri("https://huggingface.co/mcp"),
        AdditionalHeaders = hfHeaders
    });
await using var mcpClient = await McpClientFactory.CreateAsync(clientTransport);

// Display the available server tools
var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"Connected to server with the tools:{tool.Name}");
}
Console.WriteLine("Press Enter to continue:");
Console.ReadLine();
Console.WriteLine();

// Create the ChatClient
IChatClient client = GetChatClient();
var chatOptions = new ChatOptions
{
    Tools = [.. tools],
    ModelId = deploymentName
};

// create the image
Console.WriteLine("Started the process to generate an image of a pixelated puppy..");
var query = "Create an image of a pixelated puppy.";
var result = await client.GetResponseAsync(query, chatOptions);
Console.WriteLine($"AI response:{result}");
Console.WriteLine();





IChatClient GetChatClient()
{
    IChatClient client = null;
    var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? config["GITHUB_TOKEN"];
    if(!string.IsNullOrEmpty(githubToken))
    {
        client = new ChatCompletionsClient(
            endpoint: new Uri("https://models.github.ai/inference"),
            credential: new Azure.AzureKeyCredential(githubToken))
            .AsIChatClient(deploymentName)
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
    }
    else
    {
        var endpoint = config["AzureOpenAI:Endpoint"];
        var apiKey = new ApiKeyCredential(config["AzureOpenAI:ApiKey"]);

        client = new AzureOpenAIClient(new Uri(endpoint), apiKey)
            .GetChatClient(deploymentName)
            .AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
    }
    return client;
}

