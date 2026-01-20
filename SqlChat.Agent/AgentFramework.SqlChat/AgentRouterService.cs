using Azure;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using SharedLib;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AgentFramework.SqlChat;

public class AgentRouterService
{
    private readonly ChatClient _chatClient;

    public AgentRouterService(ChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<object> HandleAsync(string input)
    {
        var intentAgent = _chatClient.CreateAIAgent("IntentAgent",
            "Determine if input is Movies, Music, or Others. Respond only with JSON.");
       
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };

        var result = await intentAgent.RunAsync(input,
           options: new ChatClientAgentRunOptions
           {
               ChatOptions = new ChatOptions
               {
                   ResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat
                       .ForJsonSchema<IntentResponse>(jsonOptions)
               }
           });

        var intent = result.Deserialize<IntentResponse>(jsonOptions)!.Intent;

        var agent = intent switch
        {
            Intent.Movies => _chatClient.CreateAIAgent("MovieAgent",
                "Provide movie recommendations under 200 characters."),
            Intent.Music => _chatClient.CreateAIAgent("MusicAgent",
                "Provide music recommendations under 200 characters."),
            _ => _chatClient.CreateAIAgent("OthersAgent",
                "Provide general recommendations under 200 characters.")
        };

        var final = await agent.RunAsync(input);
       
        return new
        {
            Intent = intent.ToString(),
            Response = final.ToString(),
            InputTokenUsed= final.Usage?.InputTokenCount,
            OutputTokenUsed = final.Usage?.OutputTokenCount,
            OutputTokensUsedForReasoning = final.Usage?.GetOutputTokensUsedForReasoning()
        };
    }
}
