using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OllamaSharp;
using OpenAI;
using OpenAI.Embeddings;
using System.ClientModel;

enum Model
{
    None,
    Ollama,
    AzureOpenAI,
    OpenAI
}
public class Program
{
    private static async Task Main(string[] args)
    {
        Console.Clear();
        Console.WriteLine("Hello, Embedding!");

        while(true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Please select the Model");
            Console.WriteLine("-------------------------");
            Console.WriteLine("1.Ollama");
            Console.WriteLine("2.Azure OpenAI");
            Console.WriteLine("3.OpenAI");
            Console.WriteLine("4.Exit");

            var model = Console.ReadLine();
            if(model == "4")
            {
                return;                
            }
            if(!Enum.TryParse<Model>(model, out var selectedModel))
            {
                Console.WriteLine("Invalid model selection");
                continue;
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Please enter the message to generate embeddings for:");
            var userMessage = Console.ReadLine();
            IEmbeddingGenerator<string, Embedding<float>>? generator =null;

            switch (selectedModel)
            {
                case Model.Ollama:
                     generator = GetOllamaEmbeddingGenerator();

                    Console.ForegroundColor = ConsoleColor.Green;

                    if (generator != null && userMessage != null)
                    {
                        var embedding = await generator.GenerateAsync([userMessage]);
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine(string.Join(",", embedding[0].Vector.ToArray()));
                    }
                    else
                    {
                        Console.WriteLine("Failed to create embedding generated");
                    }
                    break;

                case Model.AzureOpenAI:
                    Console.ForegroundColor = ConsoleColor.Green;
                    var azureOpenAIGenerator = GetAzureOpenAIEmbeddingGenerator();
                    if (azureOpenAIGenerator != null && userMessage != null)
                    {
                        var response = await azureOpenAIGenerator?.GenerateEmbeddingAsync(userMessage);
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine(string.Join(",", response));
                    }
                    break;

                case Model.OpenAI:
                    Console.ForegroundColor = ConsoleColor.Green;
                    var openAIGenerator = GetOpenAIEmbeddingGenerator();
                    if(openAIGenerator != null && userMessage != null)
                    {
                        var response = await openAIGenerator?.GenerateEmbeddingsAsync([userMessage]);
                        Console.ForegroundColor= ConsoleColor.DarkGreen;

                        foreach (OpenAIEmbedding embd in response.Value)
                        {
                            ReadOnlyMemory<float> vector = embd.ToFloats();
                            int length = vector.Length;
                            System.Console.Write($"data[{embd.Index}]: length={length}, ");
                            System.Console.Write($"[{vector.Span[0]}, {vector.Span[1]}, ..., ");
                            System.Console.WriteLine($"{vector.Span[length - 2]}, {vector.Span[length - 1]}]");

                            Console.WriteLine(string.Join(",", vector.ToArray()));
                        }
                    }
                    break;
            }          
            
        }
        Console.Read();
    }

    private static IEmbeddingGenerator<string, Embedding<float>>? GetOllamaEmbeddingGenerator()
    {
        var ollama = new OllamaApiClient("http://localhost:11434", "mxbai-embed-large");
        return ollama;
    }

    public static EmbeddingClient? GetAzureOpenAIEmbeddingGenerator()
    {
        var key = Environment.GetEnvironmentVariable("GitHubToken", EnvironmentVariableTarget.User);

        // Ensure the key is not null before using it
        if (string.IsNullOrEmpty(key))
        {
            Console.WriteLine("API key is missing. Please set the 'GitHubToken' environment variable.");
            return null;
        }

        var azureClient = new AzureOpenAIClient(
            new Uri("https://models.inference.ai.azure.com"),
            new ApiKeyCredential(key));

        // Correct usage: Use GetEmbeddingClient to retrieve the embedding client
        var embeddingClient = azureClient.GetEmbeddingClient("text-embedding-3-small");
        return embeddingClient;
    }

    public static EmbeddingClient GetOpenAIEmbeddingGenerator()
    {
        var key = Environment.GetEnvironmentVariable("OpenAIToken", EnvironmentVariableTarget.User);
        
        if (string.IsNullOrEmpty(key))
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            key = config["OpenAIToken"];
        }
        
        if (string.IsNullOrEmpty(key))
        {
            Console.WriteLine("API key is missing. Please set the 'OpenAIToken' environment variable or add it to secrets.json.");
            return null;
        }
        
        var openAIClient = new OpenAIClient(new ApiKeyCredential(key));
        return openAIClient.GetEmbeddingClient("text-embedding-3-small");
    }
    

}