using BlazorHybridSearchApp.Components;
using BlazorHybridSearchApp.Models;
using BlazorHybridSearchApp.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
ConfigurationManager config = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddQdrantClient("qdrant");

var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.Services.AddQdrantVectorStore("localhost");


// Register the Ollama Embedding Generator
string modelId = config["LLMSettings:OllamaEmbeddingModelId"] ?? "llava";
Uri endpoint = new Uri(config["LLMSettings:OllamaEndpoint"] ?? "http://localhost:11434");

builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(
    sp => new OllamaEmbeddingGenerator(endpoint, modelId));

builder.Services.AddSingleton<IDataService, DataService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
