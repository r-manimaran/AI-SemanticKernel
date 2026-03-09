using AgentFrameworkMCPDockerTools;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI.Chat;
using System.Text;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(LLMConfig.Endpoint),
                            new System.ClientModel.ApiKeyCredential(LLMConfig.ApiKey));

// Create the MCP Client
McpClient dockerMcpClient = await McpClient.CreateAsync(new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint = new Uri("http://localhost:5000/mcp/"),
    TransportMode = HttpTransportMode.StreamableHttp,
}));

IList<McpClientTool> toolInDockerMcp = await dockerMcpClient.ListToolsAsync();

AIAgent agent = client.GetChatClient(LLMConfig.DeploymentOrModelId)
    .AsAIAgent(
    instructions: "You are a Docker expert. Analyze the user query and use appropriate tools from the MCP",
    tools: toolInDockerMcp.Cast<AITool>().ToList()
    ).AsBuilder()
    .Use(FunctionCallingMiddleware)
    .Build();

while (true)
{
    Console.Write("User:>");
    string userInput = Console.ReadLine() ?? string.Empty;
    if (string.IsNullOrEmpty(userInput))
    {
        break;
    }
    var userMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, userInput);
    AgentResponse response = await agent.RunAsync(userMessage);
    Console.WriteLine($"Agent:>{response}");
    Utils.Separator();
}

async ValueTask<object?> FunctionCallingMiddleware(AIAgent callingAgent, FunctionInvocationContext context,
    Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
    CancellationToken token)
{
    StringBuilder functionCallDetails = new();
    functionCallDetails.Append($"-Tool Call: '{context.Function.Name}'");
    if (context.Arguments != null && context.Arguments.Count > 0)
    {
        functionCallDetails.AppendLine();
        functionCallDetails.AppendLine("-With Arguments:");
        functionCallDetails.AppendLine(string.Join(Environment.NewLine, context.Arguments.Select(kv => $"  - {kv.Key}: {kv.Value}")));
    }
    Utils.WriteLineInformation(functionCallDetails.ToString());
    try
    {
        return await next(context, token);
    }
    catch (Exception ex)
    {
        Utils.WriteLineError($"Tool call failed: {ex.Message}");
        throw;
    }
}