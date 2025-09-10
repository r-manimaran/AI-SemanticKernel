using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

namespace WebApi.Models;

public record CustomerSupportTicket
{
    [VectorStoreKey]
    public string TicketId { get; set; } = string.Empty;
    [VectorStoreData]
    public string CustomerName { get; set; } = string.Empty;
    [VectorStoreData]
    [TextSearchResultValue]
    public string CustomerEmail { get; set; }
    [VectorStoreData]
    [TextSearchResultName]
    public string IssueDescription { get; set; } = string.Empty;
    [VectorStoreData]
    public string Status { get; set; } = "Open"; // Open, In Progress, Resolved, Closed
    [VectorStoreData]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent
    [VectorStoreData]
    public DateTime CreatedAt { get; set; }
    [VectorStoreData]
    public DateTime? ResolvedAt { get; set; }
    [VectorStoreData]
    public int SatisfactionRating { get; set; } // 1 to 5
    [VectorStoreData]
    public string? ResolutionNotes { get; set; }

    [VectorStoreVector(Dimensions:768, DistanceFunction =DistanceFunction.CosineDistance)]
    public ReadOnlyMemory<float>? Embedding { get; set; } = null;
}
