using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SqlChat.Agent;

public class SqlChatAgent : ISqlChatAgent
{
    private readonly Kernel _kernel;
    private readonly ILogger<SqlChatAgent> _logger;
    private readonly IChatCompletionService _chat;
    private readonly string _systemPrompt;

    public SqlChatAgent(Kernel kernel, ILogger<SqlChatAgent> logger)
    {
        _kernel = kernel;
        _logger = logger;
        _chat = kernel.GetRequiredService<IChatCompletionService>();
        _systemPrompt = File.ReadAllText("SqlSystemPrompt.txt");
    }

    public async Task<string> AskAsync(string question)
    {
        var history = new ChatHistory(_systemPrompt);
        history.AddUserMessage(question);
        _logger.LogInformation("Question:{question}", question);

        var response = await _chat.GetChatMessageContentAsync(history,
            new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            },
            _kernel);
        return response.Content!;
    }
}
