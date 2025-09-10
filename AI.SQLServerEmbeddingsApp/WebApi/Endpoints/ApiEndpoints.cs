using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.SqlServer;
using WebApi.Models;
using Microsoft.SemanticKernel.Data;

namespace WebApi.Endpoints;

public static class ApiEndpoints
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("Tickets").WithTags("Tickets");

        app.MapPost("/Generate", async (IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, IConfiguration configuration, ILogger<Program> logger) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var collection = new SqlServerCollection<string, CustomerSupportTicket>(connectionString, "CustomerSupportTickets");
            await collection.EnsureCollectionExistsAsync();
            logger.LogInformation("Starting data generation...");

            var tickets = TicketsService.GetCustomerSupportTickets();

            foreach (var item in tickets)
            {
                try
                {
                    var embedding = await embeddingGenerator.GenerateVectorAsync(item.IssueDescription);

                    var data = item with
                    {
                        Embedding = embedding
                    };
                    await collection.UpsertAsync(data);
                }
                catch (InvalidCastException ex)
                {
                    logger.LogError(ex, "Error generating embedding for ticket {TicketId}", item.TicketId);
                    // Log all property values to identify the problematic field
                    logger.LogError("Ticket data - TicketId: {TicketId} (Type: {TicketIdType}), " +
                       "CustomerName: {CustomerName} (Type: {CustomerNameType}), " +
                       "SatisfactionRating: {SatisfactionRating} (Type: {SatisfactionRatingType})",
                        item.TicketId, item.TicketId.GetType().Name,
                        item.CustomerName, item.CustomerName.GetType().Name,
                        item.SatisfactionRating, item.SatisfactionRating.GetType().Name);
        
                    throw; // Re-throw to see full stack trace
                }
            }
            logger.LogInformation("Data generation completed.");
            return Results.Ok(tickets);
        });

        app.MapGet("/Search", async (IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, IConfiguration configuration, string query, int topK=1) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var collection = new SqlServerCollection<string, CustomerSupportTicket>(connectionString, "CustomerSupportTickets");
            await collection.EnsureCollectionExistsAsync();
            var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var ts = new VectorStoreTextSearch<CustomerSupportTicket>(collection, embeddingGenerator);
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            var result = await ts.GetTextSearchResultsAsync(query, new TextSearchOptions() { Top = 1 });
            Dictionary<string, string> results = new();
            await foreach(var r in result.Results)
            {
                results.Add(r.Value, r.Name);
            }
            return Results.Ok(results);

        });
    }
}
