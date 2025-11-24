using Azure.AI.OpenAI;
using ClientApp_MicrosftAgentFramework;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;
using System.Text;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(LLMConfig.Endpoint), new System.ClientModel.ApiKeyCredential(LLMConfig.ApiKey));

McpClient weatherClient = await McpClient.CreateAsync(new HttpClientTransport(new HttpClientTransportOptions
{

    Endpoint = new Uri("http://localhost:5080"),
    TransportMode = HttpTransportMode.StreamableHttp,   
    AdditionalHeaders = new Dictionary<string, string>
    {
        {"Authorization","KEY-Xyz-1234" }
    },
    
}));

IList<McpClientTool> toolInWeatherMcp = await weatherClient.ListToolsAsync();

AIAgent agent = client.GetChatClient(LLMConfig.DeploymentOrModelId)
    .CreateAIAgent(instructions: "You are a weather expert for the cities",
    tools: toolInWeatherMcp.Cast<AITool>().ToList())
    .AsBuilder()
    .Use(FunctionCallingMiddleware).Build();

AgentThread thread = agent.GetNewThread();
while (true)
{
    Console.WriteLine("User >");
    string userInput = Console.ReadLine() ?? string.Empty;
    if(string.IsNullOrEmpty(userInput))
    {
        break;
    }

    ChatMessage userMessage = new ChatMessage(ChatRole.User, userInput);
    AgentRunResponse response = await agent.RunAsync(userMessage, thread);

    Console.WriteLine($"Agent:> {response}");
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
    return await next(context, token);
}