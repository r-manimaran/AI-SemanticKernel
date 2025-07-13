using Qdrant.Client;
using WebApi.Infrastructure;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Endpoints
{
    public static class RAGEndpoints
    {

        public static void MapRAGEndpoints(this IEndpointRouteBuilder route)
        {
            var app = route.MapGroup("/api/rag").WithOpenApi();

            // Get the Products
            app.MapGet("/products", (ProductService productService, ILogger<Program> logger) =>
            {
                logger.LogInformation("Getting the products!");

                var products = productService.GetAllProducts();

                return Results.Ok(products);
            }).WithName("GetProducts")
              .WithTags("RAG");

            // Initialize the RAG
            app.MapPost("/initialize", async (ProductService productService, QdrantIndexer indexer,ILogger<Program> logger) =>
            {
                logger.LogInformation( "Initializing RAG with products...");
                try
                {
                    await indexer.InitializeAsync();
                    return Results.Ok("Successfully initialized");
                }
                catch (Exception ex)
                {

                    return Results.Problem($"Error initializing collection:{ex.Message}");
                }
            }).WithName("Initialize");

            app.MapPost("/query", async (
                UserQuery userQuery,
                QdrantClient client,
                IEmbeddingService embeddingService,
                ChatService chatService,
                ILogger<Program> logger) =>
            {
                logger.LogInformation("Processing user query: {Query}", userQuery.Query);
                var embedding = await embeddingService.GetEmbeddingAsync(userQuery.Query);
                logger.LogInformation("Search in Qdrant for the result for your query");
                var result = await client.SearchAsync("products", embedding, limit: 3);

                var context = result.Any()
                    ? string.Join("\n", result.Select(r => r.Payload["name"].StringValue))
                    : "No relevant context found.";

                logger.LogInformation("Context for the query: {Context}", context);
                var prompt = $"Using the following context, answer the question: {userQuery.Query}\nContext: {context}";

                var answer = await chatService.GetChatResponseAsync(prompt);
                return Results.Ok(answer);
            })
            .WithName("RAGQuery");
      

         
        }
    }
}
