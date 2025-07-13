using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace WebApi.Infrastructure;

public class ChatService(Kernel kernel, ILogger<ChatService> logger)
{
    public async Task<string> GetChatResponseAsync(string query)
    {
        // Log the incoming query
        logger.LogInformation("Received query: {Query}", query);
        var chat = kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory();
        history.AddUserMessage(query);

        var answer = new StringBuilder();
        var result = chat.GetStreamingChatMessageContentsAsync(history);
        await foreach (var content in result)
        {
            // Log each chunk of the response
            logger.LogInformation("Received chunk: {Content}", content);
            answer.Append(content);
        }

        history.AddAssistantMessage(answer.ToString());
        // Log the final answer
        logger.LogInformation("Final answer: {Answer}", answer.ToString());
        return answer.ToString();
    }
}
