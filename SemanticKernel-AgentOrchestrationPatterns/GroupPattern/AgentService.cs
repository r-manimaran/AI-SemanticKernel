using BaseKernel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupPattern;

public class AgentService
{
    List<ChatMessageContent> chatHistory = new List<ChatMessageContent>();
    public async Task RunAsync(ILogger logger)
    {
        logger.LogInformation("Agent - GroupChat Orchestration Pattern");
        
        Kernel kernel = BaseKernelFactory.OpenAIKernel();

        ChatCompletionAgent softwareAgent = CreateSoftwareAgent(kernel);

        ChatCompletionAgent qaTesterAgent = CreateQAAgent(kernel);

        ChatCompletionAgent codeReviewAgent = CreateCodeReviewAgent(kernel);

        //  GroupChatOrchestration groupChatOrchestration = new(new RoundRobinGroupChatManager()
        GroupChatOrchestration groupChatOrchestration = new(new SmartRoundRobinGroupChatManager()
        {
            MaximumInvocationCount = 5,
            InteractiveCallback = InteractiveCallback
        }, softwareAgent, qaTesterAgent, codeReviewAgent)
        {
            ResponseCallback = ResponseCallback
        };
        InProcessRuntime inProcessRuntime = new InProcessRuntime();

        await inProcessRuntime.StartAsync();

        var prompt = "Write a C# method to calculate the factorial of a number";

        OrchestrationResult<string> orchestrationResult = await groupChatOrchestration.InvokeAsync(prompt, inProcessRuntime);

        string output = await orchestrationResult.GetValueAsync();

        await inProcessRuntime.RunUntilIdleAsync();

        logger.LogInformation("Orchestration Result:{result}",output);

        logger.LogInformation("ChatHistory:");

        foreach (var message in chatHistory)
        {
            logger.LogInformation($"Agent:{message.AuthorName}");

            logger.LogInformation($"Result: {message.Content}");
            
            logger.LogInformation("--------------------------");
        }

        Console.Read();
    }

    private async ValueTask<ChatMessageContent> InteractiveCallback()
    {
        //logger.LogInformation("Interactive callback triggered. Please provide your input:");

        string userInput = Console.ReadLine() ?? string.Empty;
        
        ChatMessageContent userMessage = new ChatMessageContent(AuthorRole.User, userInput);

        return await ValueTask.FromResult(userMessage);
    }

    private ValueTask ResponseCallback(ChatMessageContent response)
    {
        chatHistory.Add(response);

        return ValueTask.CompletedTask;
    }

    private static ChatCompletionAgent CreateSoftwareAgent(Kernel kernel)
    {
        ChatCompletionAgent softwareEngineerAgent = new ChatCompletionAgent
        {
            Name ="SoftwareEngineerAgent",
            Description ="Implements features based on product requirements.",
            Instructions = """
            You are a senior software engineer. Given a feature request, write a clear and simple code snippet in C#
            to implement the functionality. Focus on correctness and readability. Return only one implementation
            per response. No extra comments or chit-chat.
            """,
            Kernel = kernel.Clone()
        };
        return softwareEngineerAgent;
    }

    private static ChatCompletionAgent CreateQAAgent(Kernel kernel)
    {
        ChatCompletionAgent QAagent = new ChatCompletionAgent
        {
            Name = "QATesterAgent",
            Description = "Tests and validates software feature implementations.",
            Instructions = """
                        You are a meticulous QA tester. Analyze the provided code to determine if it meets the feature requirements.
                        If it works as intended, state that it is approved for release.
                        If not, identify functional issues or edge cases that the implementation misses.
                        Do not provide code-only feedback.
                        """,
            Kernel = kernel.Clone()
        };
        return QAagent;
    }

    private static ChatCompletionAgent CreateCodeReviewAgent(Kernel kernel)
    {
        ChatCompletionAgent codeReviewAgent = new ChatCompletionAgent
        {
            Name = "codeReviewAgent",
            Description = "Review code quality and adherance to best practices",
            Instructions = """
            You are an experienced software engineer specializing in code reviews.
            Review the given code snippet for style, maintainability, readability, and adherance to best practices.
            Provide constructive feedback and suggestions for improvement. but do not rewrite the code.
            """,
            Kernel = kernel.Clone()
        };
        return codeReviewAgent;
    }
}

sealed class SmartRoundRobinGroupChatManager : RoundRobinGroupChatManager
{
    public override ValueTask<GroupChatManagerResult<bool>> ShouldRequestUserInput(ChatHistory history, CancellationToken cancellationToken = default)
    {
        var isApproved = history.Last().Content.Contains("approve");
        if (isApproved)
        {
            return ValueTask.FromResult(new GroupChatManagerResult<bool>(true) { Reason = "User approval is required." });
        }

        return ValueTask.FromResult(new GroupChatManagerResult<bool>(false) { Reason = "No user approval is required." });
    }
}