using BlazorHybridSearchApp.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using System.Text.Json;

namespace BlazorHybridSearchApp.Services;

public class DataService : IDataService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly IConfiguration _configuration;
    private readonly QdrantCollection<Guid, RestaurantDataForVector> _vectorStoreRecordCollection;

    public DataService(QdrantClient client,
                        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
                        IConfiguration configuration)
    {
        _embeddingGenerator = embeddingGenerator;
        _configuration = configuration;
        _vectorStoreRecordCollection = new QdrantCollection<Guid, RestaurantDataForVector>(client,"restaurants",ownsClient:true);

        var coExists = _vectorStoreRecordCollection.CollectionExistsAsync().Result;
        if (coExists)
        {
            _vectorStoreRecordCollection.EnsureCollectionDeletedAsync().Wait();
        }

        _vectorStoreRecordCollection.EnsureCollectionExistsAsync().Wait();
    }

    public async Task LoadData(string filePath)
    {
        var jsonData = await File.ReadAllTextAsync(filePath);
        var restaurantDataList = JsonSerializer.Deserialize<List<RestaurantData>>(jsonData);
        if (restaurantDataList == null || !restaurantDataList.Any())
        {
            throw new InvalidOperationException("No restaurant data found in the provided file.");
        }

        var tasks = restaurantDataList.Select(async restaurantData => new RestaurantDataForVector
        {
            Key = Guid.NewGuid(),
            Name = restaurantData.Name,
            Description = restaurantData.Description,
            Location = restaurantData.Location,
            DescriptionEmbedding = await _embeddingGenerator.GenerateVectorAsync(restaurantData.Description).ConfigureAwait(false),
           // Features = await _embeddingGenerator.GenerateVectorAsync(restaurantData.Features).ConfigureAwait(false)

        });

        var data = await Task.WhenAll(tasks).ConfigureAwait(false);
        await _vectorStoreRecordCollection.UpsertAsync(data);
    }

    public async Task<List<string>> GetRestaurantInfo(string query, int topK = 5)
    {
        List<string> response = [];
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Query cannot be null or empty.", nameof(query));
        }
        // Generate the embedding for the query
        var queryEmbedding = await _embeddingGenerator.GenerateVectorAsync(query);

        var collection = (IKeywordHybridSearchable<RestaurantDataForVector>)_vectorStoreRecordCollection;

        HybridSearchOptions<RestaurantDataForVector> hybridSearchOptions = new()
        {
            VectorProperty = x => x.DescriptionEmbedding,
            AdditionalProperty = x => x.Location,
        };

        var searchResults = collection.HybridSearchAsync(queryEmbedding, [], topK);

        await foreach (var result in searchResults)
        {
            response.Add($"Restaurant: {result.Record.Name}, " +
                         $"Description: {result.Record.Description}, " +
                         $"Location: {result.Record.Location}, " +
                         $"Score:{result.Score}");
        }
        return response;
    }
}
