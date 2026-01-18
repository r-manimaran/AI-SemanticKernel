namespace SqlChat.Agent;

public interface ISqlChatAgent
{
    Task<string> AskAsync(string question);
}
