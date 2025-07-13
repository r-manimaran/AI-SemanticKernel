using Qdrant.Client;
using Qdrant.Client.Grpc;
using WebApi.Services;

namespace WebApi.Infrastructure;

public class QdrantIndexer(QdrantClient client,
                    ProductService productService,
                    IEmbeddingService embeddingService,
                    ILogger<QdrantIndexer> logger)
{
    public async Task InitializeAsync()
    {
        logger.LogInformation("Initializing Qdrant indexer...");

        try
        {
            // Get the List of Collections from the Qdrant
            var collections = await client.ListCollectionsAsync();

            // Check if the 'products' collection exists
            if (!collections.Contains("products"))
            {
                // text-embedding-3-small
                await client.CreateCollectionAsync("products", new VectorParams { Size = 1536, Distance = Distance.Cosine });

                // Create a new collection if it does not exist
                // using embedding size of 384
               // await client.CreateCollectionAsync("products", new VectorParams { Size = 384, Distance = Distance.Cosine });

                logger.LogInformation("Created 'products' collection in Qdrant.");

            }
            else
            {
                logger.LogInformation("'products' collection already exists in Qdrant.");
            }

            var embeddings = await Task.WhenAll(
                    productService.GetAllProducts().Select(async product =>

                    new PointStruct
                    {

                        Id = new PointId { Num = (ulong)product.Id },

                        Vectors = new Vectors()
                        {
                            Vector = new Vector
                            {
                                Data =
                                {
                                    (await embeddingService.GetEmbeddingAsync(product.Description)).ToArray()
                                }
                            }
                        },

                        Payload =
                        {
                            { "id", new Value { IntegerValue = product.Id } },
                            { "name", new Value { StringValue = product.Name } },
                            { "description", new Value { StringValue = product.Description } }
                        }
                    }));

            const int batchSize = 100;
            for (int i = 0; i < embeddings.Length; i += batchSize)
            {
                var batch = embeddings.Skip(i).Take(batchSize).ToList();
                await client.UpsertAsync("products", batch);
                logger.LogInformation($"Inserted {batch.Count} products into Qdrant.");
            }
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing collections in Qdrant");
            throw;

        }
    }
}
