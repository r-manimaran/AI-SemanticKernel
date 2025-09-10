namespace WebApi.Appsettings;

public class AISettings
{
    public static string SectionName => "AISettings";
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
    public string ChatModel { get; set; } = "gpt-4o";
    public string OllamaEndpoint { get; set; } = "http://localhost:11434";
}
