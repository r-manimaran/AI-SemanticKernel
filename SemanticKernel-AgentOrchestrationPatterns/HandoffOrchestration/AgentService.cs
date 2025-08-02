using BaseKernel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration.Handoff;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandoffOrchestrationApp;

public class AgentService
{
    List<ChatMessageContent> chatHistory = new List<ChatMessageContent>();
    public async Task RunAsync(ILogger<AgentService> logger, string task)
    {
        Kernel kernel = BaseKernelFactory.OpenAIKernel();

        ChatCompletionAgent triageAgent = CreateTriageAgent(kernel);

        ChatCompletionAgent statusAgent = CreateStatusAgent(kernel);

        ChatCompletionAgent refundAgent = CreateRefundAgent(kernel);

        ChatCompletionAgent returnAgent = CreateReturnAgent(kernel);

        statusAgent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromObject(new OrderStatusPlugin()));

        refundAgent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromObject(new OrderRefundPlugin()));

        returnAgent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromObject(new OrderReturnPlugin()));

        var handOffs = OrchestrationHandoffs
                        .StartWith(triageAgent)
                        .Add(triageAgent, statusAgent, refundAgent, returnAgent)
                        .Add(statusAgent, triageAgent, "Transfer to triage if the issue is not status related.")
                        .Add(returnAgent, triageAgent, "Transfer to triage if the issue is not return related.")
                        .Add(refundAgent, triageAgent, "Transfer to triage if the issue is not refund related.");

        HandoffOrchestration orchestration = new HandoffOrchestration(handOffs,
            triageAgent,
            statusAgent,
            returnAgent,
            refundAgent)
        {
            ResponseCallback = (content) => ResponseCallback(content, logger),
            InteractiveCallback = () => InteractiveCallback(logger)

        };

        InProcessRuntime inProcessRuntime = new InProcessRuntime();

        await inProcessRuntime.StartAsync();

        //string task = "I need help with my order 12345. Can you tell me its status?";
        
        var result = await orchestration.InvokeAsync(task,inProcessRuntime);

        string finalOutput = await result.GetValueAsync();

        logger.LogInformation($"Final output:{finalOutput}");

        await inProcessRuntime.StopAsync();

    }

    private async ValueTask<ChatMessageContent> InteractiveCallback(ILogger<AgentService> logger)
    {
        logger.LogInformation("# User Input Required:");
        string userInput = Console.ReadLine()!;
        var message = new ChatMessageContent(AuthorRole.User, userInput);
        chatHistory.Add(message);
        return message;
    }

    private ValueTask ResponseCallback(ChatMessageContent response, ILogger<AgentService> logger)
    {
        chatHistory.Add(response);
        logger.LogInformation($"Agent {response.AuthorName}:{response.Content}");
        return ValueTask.CompletedTask;
    }

    private ChatCompletionAgent CreateReturnAgent(Kernel kernel)
    {
        ChatCompletionAgent returnAgent = new ChatCompletionAgent()
        {
            Name = "ReturnAgent",
            Description = "Handle the Return Requests on the orders",
            Instructions = """
            You handle return requests. Use the OrderReturnPlugin to process returns.
            """,
            Kernel = kernel.Clone()
        };
        return returnAgent;
    }

    private ChatCompletionAgent CreateRefundAgent(Kernel kernel)
    {
        ChatCompletionAgent refundAgent = new ChatCompletionAgent()
        {
            Name = "RefundAgent",
            Description = "Handle the Refund Requests on the orders.",
            Instructions = """
            You handle refund requests. Use the OrderRefundPlugin to process refunds.
            """,
            Kernel = kernel.Clone(),
        };
        return refundAgent;
    }

    private ChatCompletionAgent CreateStatusAgent(Kernel kernel)
    {
        ChatCompletionAgent statusAgent = new ChatCompletionAgent()
        {
            Name="OrderStatusAgent",
            Description="Handle the status on the orders placed.",
            Instructions="""
            You handle order status inquiries. Use the OrderStatusPlugin to check the status.
            """,
            Kernel = kernel.Clone(),
        };
        return statusAgent;
    }

    private ChatCompletionAgent CreateTriageAgent(Kernel kernel)
    {
        ChatCompletionAgent triageAgent = new ChatCompletionAgent()
        {
            Name = "TriageAgent",
            Description = "Handle the initial triage on the incidents received related to Orders.",
            Instructions = """
            You are a custom support triage agent. Analyze the user's request and determine if it relates to order status, refunds, or returns.
            Handoff to the appropriate agent or request human input if unclear.
            """,
            Kernel = kernel.Clone()            
        };
        return triageAgent;
    }


}
