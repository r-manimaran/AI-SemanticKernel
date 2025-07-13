using Microsoft.Extensions.AI;

namespace WebApi.Services;

public class EmbeddingService(IEmbeddingGenerator<string,Embedding<float>> embeddingGenerator) : IEmbeddingService
{
    public async Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string text)
    {
        return await embeddingGenerator.GenerateVectorAsync(text);
    }
}

public interface IEmbeddingService
{
    
    Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string text);
}
