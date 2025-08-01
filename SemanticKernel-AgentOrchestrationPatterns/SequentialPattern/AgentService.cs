﻿using BaseKernel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequentialPattern;

public class AgentService
{
    public async Task RunAsync(ILogger logger)
    {
        logger.LogInformation("Agent Sequential Orchestration Pattern");

        logger.LogInformation("Using OpenAI");
        var kernel = BaseKernelFactory.OpenAIKernel();

        //logger.LogInformation("Using Azure OpenAI");
        //var kernel = BaseKernelFactory.AzureOpenAIKernel();

        ChatCompletionAgent intentAgent = CreateIntentAgent(kernel);

        ChatCompletionAgent emailGenerateAgent = CreateEmailGenerationAgent(kernel);

        ChatCompletionAgent toneAgent = CreateToneAnalysisAgent(kernel);

        // Added in the Project Properties SKEXP0110
        SequentialOrchestration sequentialOrchestration = new SequentialOrchestration(intentAgent, emailGenerateAgent, toneAgent)
        {
            ResponseCallback = content=> ResponseCallback(content, logger)
        };

        var input = "Thank a friend and confirm attendance at a birthday party";

        InProcessRuntime inProcessRuntime = new InProcessRuntime();
        await inProcessRuntime.StartAsync();

        var result = await sequentialOrchestration.InvokeAsync(input, inProcessRuntime);

        var finalResult = await result.GetValueAsync();

        logger.LogInformation($"Final Result:{finalResult}");

        await inProcessRuntime.RunUntilIdleAsync();
    }
    private static ChatCompletionAgent CreateIntentAgent(Kernel kernel)
    {
        return new ChatCompletionAgent
        {
            Name = "IntentAgent",
            Description = "Extracts the user's intent from input text.",
            Instructions = """
            You are an intent extraction agent. Read the user's input and identify their core intent.
            Respond strictly in the Json Format. \n
            { "intent": "[brief description of what the user wants to achieve] }
            This output will be passwed directly to the EmailAgent for composing an email.
            """,
            Kernel = kernel.Clone()
        };
    }
    private static ChatCompletionAgent CreateEmailGenerationAgent(Kernel kernel)
    {
        return new ChatCompletionAgent
        {
            Name = "EmailAgent",
            Description = "Compose a friendly and polite email based on the users intent",
            Instructions = """            
            You are an Email Composer Agent. Your task is to generate a short, friendly and polite email based on the user's intent.
            { "intent":"Thank a friend and confirm attendance at a birthday party" }
            Compose a natural and human-sounding email reflectig this intent.
            Do not add greetings like 'Dear Assistant'. Only output the email body.
            This email will be set to the ToneAgent for review.            
            """,
            Kernel = kernel
        };
    }
    private static ChatCompletionAgent CreateToneAnalysisAgent(Kernel kernel)
    {
        return new ChatCompletionAgent
        {
            Name = "ToneAgent",
            Description = "Checks and edits the tone of the email to ensure it is friendly and polite",
            Instructions = """
            You are a tone checker and Editor Agent.You will receive an email content generated by the Email Agent.
            Your job is to check if the tone is friendly and polite. If the tone is fine, respond with: The tone is friendly
            and polite. No changes is needed. If the tone can be improved, revise the email and return the corrected version
            prefixed with: Corrected Email: \n\n Only output the final result-no explanations.
            """,
            Kernel = kernel.Clone()
        };
    }

    private static ValueTask ResponseCallback(ChatMessageContent content, ILogger logger)
    {
        //SKEXP0001
        var resultCallback = $"Agent Name: {content.AuthorName} \n Role:{content.Role} \n Output: {content.Content}";

        //Console.WriteLine(resultCallback);
        logger.LogInformation(resultCallback);

        return ValueTask.CompletedTask;
    }

}
