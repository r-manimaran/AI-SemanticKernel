using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Threading.Tasks;

namespace SK_MultiAgent;
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var kernelBuilder = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(Config.DeploymentOrModelId,Config.ApiKey).Build();
                                        
            //.AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId,
            //                              Config.Endpoint, 
            //                              Config.ApiKey).Build();

        // Define the Agents
        var mathsAgent = CreateMathAgent(kernelBuilder);

        var englishAgent = CreateEnglishAgent(kernelBuilder);

        var principalAgent = CreatePrincipalAgent(kernelBuilder);


        AgentGroupChat chat =
            new AgentGroupChat(englishAgent, mathsAgent, principalAgent)
            {
                ExecutionSettings =
                new AgentGroupChatSettings()
                {
                    TerminationStrategy = new ApprovalTerminationStrategy()
                    {
                        Agents = [principalAgent],
                        MaximumIterations = 5,
                    }
                }
            };


        chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, content: "Semantic kernel is a powerful tool for natual languge processing, enabling more acurate" +
            "understandng of context and meaning.."));
        chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, content:"Please proide the sum of 12 and 15."));

        await foreach(var content in chat.InvokeAsync())
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n");
            if(content is null)
            {
                Console.WriteLine("No content received from the agent.");
                continue;
            }
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            string author = content.AuthorName ?? "Unknown";

            Console.ForegroundColor = content?.AuthorName switch
            {
                "Principal" => ConsoleColor.Red,
                "EnglishTeacher" => ConsoleColor.Blue,
                "MathTeacher" => ConsoleColor.Green,
                _=> Console.ForegroundColor,
            };
            Console.WriteLine($"# {content.Role} -{content.AuthorName ?? "*"}: '{content.Content}'");
        }
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        Console.WriteLine($"# IS COMPLETE: {chat.IsComplete}");
        Console.Read();
    }

  

    private static ChatCompletionAgent CreatePrincipalAgent(Kernel kernelBuilder)
    {
        string principalAgentPrompt = @"You are Principal agent. You should receive complete user information and 
                                        Your final task is to approve the results evaulated by the English and Math teachers.";

        ChatCompletionAgent principalAgent = new ChatCompletionAgent
        {
            Kernel = kernelBuilder,
            Name = "Principal",
            Instructions = principalAgentPrompt,
            Description = "Principal Agent for approving the results of English and Math teachers."
        };
    
        return principalAgent;
    }

    private static ChatCompletionAgent CreateEnglishAgent(Kernel kernelBuilder)
    {
        string englishAgentPrompt = @"You are English teacher with multiple decades of experience, known for your patience and clarity.
                    Your goal is to refine and ensure the provided text is gramattically correct and well-structured. Provide one clear and concise suggestion per response. 
                    Focus solely on improving the writing quality. Avoid unnecessay comments or corrections. Do not handle any other subject requests";

       
        ChatCompletionAgent englishAgent = new ChatCompletionAgent
        {
            Kernel = kernelBuilder,
            Name = "EnglishTeacher",
            Instructions = englishAgentPrompt,
            Description = "English Teacher Agent for refining and ensuring the provided text is grammatically correct and well-structured."
        };

        return englishAgent;
    }
    private static ChatCompletionAgent CreateMathAgent(Kernel kernelBuilder)
    {
        string matchAgentInstruction = @"You are a Math teacher with multiple decades of experience, known for your patience and clarity.
                    Your goal is to refine and ensure the provided math problem is solved correctly. Provide one clear and concise suggestion per response. 
                    Focus solely on improving the math solution. Avoid unnecessary comments or corrections. Do not handle any other subject requests.";
        
        ChatCompletionAgent mathAgent = new ChatCompletionAgent
        {
            Name = "MathTeacher",
            Kernel = kernelBuilder,
            Instructions = matchAgentInstruction,
            Description = "Math Teacher Agent for refining and ensuring the provided math problem is solved correctly."
        };

        return mathAgent;
    }

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    private sealed class ApprovalTerminationStrategy : TerminationStrategy
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    {
        protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
              => Task.FromResult(history[history.Count - 1].Content?.Contains("approve", StringComparison.OrdinalIgnoreCase) ?? false);

    }

}
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

