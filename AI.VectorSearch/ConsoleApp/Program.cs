
using ConsoleApp;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI;
using System.ClientModel;

List<CloudService> cloudServices = [

    new() {
            Key = 0,
            Name = "Azure App Service",
            Description = "Host .NET, Java, Node.js, and Python web applications and APIs in a fully managed Azure service. You only need to deploy your code to Azure. Azure takes care of all the infrastructure management like high availability, load balancing, and autoscaling."
    },
    new() {
            Key = 1,
            Name = "Azure Service Bus",
            Description = "A fully managed enterprise message broker supporting both point to point and publish-subscribe integrations. It's ideal for building decoupled applications, queue-based load leveling, or facilitating communication between microservices."
    },
    new() {
            Key = 2,
            Name = "Azure Blob Storage",
            Description = "Azure Blob Storage allows your applications to store and retrieve files in the cloud. Azure Storage is highly scalable to store massive amounts of data and data is stored redundantly to ensure high availability."
    },
    new() {
            Key = 3,
            Name = "Microsoft Entra ID",
            Description = "Manage user identities and control access to your apps, data, and resources."
    },
    new() {
            Key = 4,
            Name = "Azure Key Vault",
            Description = "Store and access application secrets like connection strings and API keys in an encrypted vault with restricted access to make sure your secrets and your application aren't compromised."
    },
    new() {
            Key = 5,
            Name = "Azure AI Search",
            Description = "Information retrieval at scale for traditional and conversational search applications, with security and options for AI enrichment and vectorization."
    }
];

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

string model = config["ModelName"] ?? "text-embedding-3-small"; 
string key = config["OpenAIKey"] ?? "REDACTED";

// Create Embedding Generator
IEmbeddingGenerator<string, Embedding<float>> generator = new OpenAIClient(new ApiKeyCredential(key))
                        .GetEmbeddingClient(model: model)
                        .AsIEmbeddingGenerator();

// Create and populate the vector store
var vectorStore = new InMemoryVectorStore();
VectorStoreCollection<int, CloudService> cloudServicesStore = vectorStore.GetCollection<int, CloudService>("cloudServices");
await cloudServicesStore.EnsureCollectionExistsAsync();

foreach (CloudService service in cloudServices)
{   
    // Generate embedding for the service description
        
    // Set the vector property
    service.Vector = await generator.GenerateVectorAsync(service.Description);

    // Add to the vector store
    await cloudServicesStore.UpsertAsync(service);
}

string query = "Which Azure service can be used to host web applications and APIs?";
//query = "Which Azure service should I use to store my Word documents?";
query = "Which Azure service can I use to manage user identities and control access to my applications?";

Console.WriteLine($"Query: {query}");
Console.WriteLine("----------------------------------------------------");
Console.WriteLine("Generating embedding for the query...");
// Generate embedding for the query
ReadOnlyMemory<float> queryEmbedding = await generator.GenerateVectorAsync(query);

Console.WriteLine("Searching for Result in CloudServiceStore...");

IAsyncEnumerable<VectorSearchResult<CloudService>> results = cloudServicesStore.SearchAsync(queryEmbedding, top: 1);

await foreach (VectorSearchResult<CloudService> result in results)
{
    Console.WriteLine($"Name: { result.Record.Name }");
    Console.WriteLine($"Description: { result.Record.Description }");
    Console.WriteLine($"Vector match score: { result.Score }");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

