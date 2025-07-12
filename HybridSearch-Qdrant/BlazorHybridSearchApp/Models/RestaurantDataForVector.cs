using Microsoft.Extensions.VectorData;
using System.Text.Json.Serialization;

namespace BlazorHybridSearchApp.Models;

public class RestaurantDataForVector
{
    [VectorStoreKey]
    public Guid Key { get; set; }
    [VectorStoreData]
    public string Name { get; set; } = string.Empty;
    [VectorStoreData]
    public string Description { get; set; } = string.Empty;
    [VectorStoreData(IsFullTextIndexed =true)]
    public string Location { get; set; } = string.Empty;
    [VectorStoreVector(4096)]
    public ReadOnlyMemory<float> DescriptionEmbedding { get; set; }
    //[VectorStoreVector(4096)]
    //public ReadOnlyMemory<float> Features { get; set; }
}

public class RestaurantData
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;
    [JsonPropertyName("features")]
    public string Features { get; set; } = string.Empty;
   
}