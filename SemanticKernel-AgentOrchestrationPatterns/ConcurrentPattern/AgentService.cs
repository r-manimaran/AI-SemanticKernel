using BaseKernel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration.Concurrent;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentPattern;

public class AgentService()
{
    List<ChatMessageContent> chatHistory = new List<ChatMessageContent>();
    public async Task RunAsync(ILogger<AgentService> logger)
    {
        logger.LogInformation("Agent Concurrent Orchestration Pattern");

        logger.LogInformation("Using OpenAI");

        var kernel = BaseKernelFactory.OpenAIKernel();

        ChatCompletionAgent actionFanAgent = CreateActionFanAgent(kernel);

        ChatCompletionAgent romanceFanAgent = CreateRomanceFanAgent(kernel);

        ChatCompletionAgent classicFanAgent = CreateClassicFanAgent(kernel);

        ChatCompletionAgent musicFanAgent = CreateMusicFanAgent(kernel);

        ChatCompletionAgent thrillerFanAgent =CreateThrillerFanAgent(kernel);

        ConcurrentOrchestration concurrentOrchestration = new ConcurrentOrchestration(
            actionFanAgent, 
            romanceFanAgent, 
            classicFanAgent, 
            musicFanAgent, 
            thrillerFanAgent)
        {
            ResponseCallback = ResponseCallback
        };

        var input = "Suggest a good tamil movie to watch for this weekend.";

        InProcessRuntime inProcessRuntime = new InProcessRuntime();

        await inProcessRuntime.StartAsync();

        var result = await concurrentOrchestration.InvokeAsync(input,inProcessRuntime);

        // In concurrent orchestration this will return the string[]
        var finalResult = await result.GetValueAsync();

        foreach (var item in finalResult)
        {
            logger.LogInformation($"{item}");
        }

        await inProcessRuntime.RunUntilIdleAsync();

        foreach (var message in chatHistory)
        {
            logger.LogInformation($"Agent:{message.AuthorName}");

            logger.LogInformation($"Result: {message.Content}");

            logger.LogInformation("---------------------------------------------");
        }
        Console.Read();
    }

    private ValueTask ResponseCallback(ChatMessageContent response)
    {
        chatHistory.Add(response);

        return ValueTask.CompletedTask;
    }

    private ChatCompletionAgent CreateClassicFanAgent(Kernel kernel)
    {
        ChatCompletionAgent chatCompletionAgent = new ChatCompletionAgent
        {
            Name = "ClassicFanAgent",
            Description = "Provides timeless classic movie recommendations known for their cultural impact and story telling",
            Instructions = """
           You are a movie lover and recommends always timeless classic movies in tamil.
           """,
            Kernel = kernel.Clone()
        };
        return chatCompletionAgent;
    }

    private ChatCompletionAgent CreateRomanceFanAgent(Kernel kernel)
    {
        ChatCompletionAgent chatCompletionAgent = new ChatCompletionAgent
        {
            Name = "RomanceFanAgent",
            Description = "Suggests feel-good romantic comedies that blend love, humor and lighthearted storytelling",
            Instructions = """
           You are a movie lover who only recommends romantic comedies and suggest feel-good rom-coms in tamil.
           """,
            Kernel = kernel.Clone()
        };
        return chatCompletionAgent;
    }

    private ChatCompletionAgent CreateActionFanAgent(Kernel kernel)
    {
        ChatCompletionAgent chatCompletionAgent = new ChatCompletionAgent
        {
            Name = "ActionFanAgent",
            Description = "Recommends high-energy action-packed movies with thrilling scenes and heroic characters.",
            Instructions = """
            You are a movie lover who only recommends top action movies in tamil.
           """,
            Kernel = kernel.Clone()
        };
        return chatCompletionAgent;
    }

    private ChatCompletionAgent CreateThrillerFanAgent(Kernel kernel)
    {
        ChatCompletionAgent chatCompletionAgent = new ChatCompletionAgent
        {
            Name = "ThrillerFanAgent",
            Description = "Suggests a spine-chilled thriller movies with jumb scars.",
            Instructions = """
            You are a movie lover who only recommends all time top spine-chilled thriller movie in tamil.
            """,
            Kernel = kernel.Clone()
        };
        return chatCompletionAgent;
    }

    private ChatCompletionAgent CreateMusicFanAgent(Kernel kernel)
    {
        ChatCompletionAgent chatCompletionAgent = new ChatCompletionAgent
        {
            Name = "MusicFanAgent",
            Description =" Recommends movies with excellent background score and super hit songs",
            Instructions = """
            You are a movie lover who only recommends tamil movie with excellent background score and super hit songs.
            """,
            Kernel = kernel.Clone()
        };
        return chatCompletionAgent;
    }
}
