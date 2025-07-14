using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;
using WebApi;
using WebApi.Endpoints;
using WebApi.Infrastructure;
using WebApi.Services;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.AddQdrantClient("qdrant");

builder.Services.AddSingleton<QdrantIndexer>();

builder.Services.AddSingleton<ChatService>();

builder.Services.AddSingleton<ProductService>();

builder.Services.AddSingleton<IEmbeddingService, EmbeddingService>();

var configuration = builder.Configuration;

builder.Services.AddSingleton<Kernel>(_ => 
{
// Return the Kernel based on the configuration
// Parse the LLMType from configuration
LLMType llmType = Enum.Parse<LLMType>(configuration["LLMType"] ?? "OPENAI");

switch (llmType)
{
        case LLMType.OPENAI:
            // using OpenAI Chat Completion and Embedding Generator
            // using OpenAI Chat Completion and Embedding Generator
             var apiKey = configuration["OpenAIKey"] ?? "ApiKeyHere";
            return Kernel.CreateBuilder()
                               .AddOpenAIChatCompletion("gpt-4", apiKey)
                               .AddOpenAIEmbeddingGenerator("text-embedding-3-small", apiKey)
                               .Build();
            
        case LLMType.AZUREOPENAI:
            return Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion("gpt-4o-mini", configuration["AzureOpenAI:Endpoint"] ?? "https://your-endpoint.openai.azure.com/", configuration["AzureOpenAI:ApiKey"] ?? "YourApiKeyHere")
                .AddAzureOpenAIEmbeddingGenerator("text-embedding-3-small", configuration["AzureOpenAI:Endpoint"] ?? "https://your-endpoint.openai.azure.com/", configuration["AzureOpenAI:ApiKey"] ?? "YourApiKeyHere")
                .Build();
            
        case LLMType.GITHUB:
            var client = new OpenAIClient(new ApiKeyCredential(configuration["GithubModel:AccessToken"] ?? ""), new OpenAIClientOptions  
            {
                Endpoint = new Uri(configuration["GithubModel:Endpoint"] ?? "https://models.inference.ai.azure.com")
            });
            return Kernel.CreateBuilder()
                .AddOpenAIChatCompletion("Phi-3.5-mini-instruct", client)
                .AddOpenAIEmbeddingGenerator("text-embedding-3-small", client)
                .Build();

        case LLMType.ONNX:
            var modelPath = configuration["OnnxModelPath"] ?? @"C:\ai-models\phi-3\models\Phi-3-mini-4k-instruct-onnx\cpu_and_mobile\cpu-int4-awq-block-128";
            return Kernel.CreateBuilder()
                        .AddOnnxRuntimeGenAIChatCompletion("phi-3-mini",modelPath)
                        .AddBertOnnxEmbeddingGenerator(
                                 onnxModelPath: @"C:\ai-models\bge-micro-v2\onnx\model.onnx",
                                 vocabPath: @"C:\ai-models\bge-micro-v2\vocab.txt")
                       .Build();
        //case LLMType.OLLAMA:
        //    return Kernel.CreateBuilder()
        //            .AddOllamaChatCompletion("gpt-3.5-turbo", "http://localhost:11434")
        //            .AddOllamaEmbeddingGenerator("text-embedding-3-small", "http://localhost:11434")
        //            .Build();
        //    break;
        default:
            throw new NotSupportedException($"LLMType '{llmType}' is not supported.");
    }    
});

builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
{
    var kernel = sp.GetRequiredService<Kernel>();

    return kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();

app.UseSwaggerUI(opt =>
    opt.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1"));

app.UseHttpsRedirection();

 app.MapRAGEndpoints();

app.Run();

