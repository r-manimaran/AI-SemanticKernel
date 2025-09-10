using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;
using WebApi.Appsettings;
using WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<AISettings>(builder.Configuration.GetSection(AISettings.SectionName));

var aiSettings = builder.Configuration.GetSection(AISettings.SectionName).Get<AISettings>();

builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp => new OllamaApiClient(aiSettings.OllamaEndpoint, aiSettings.EmbeddingModel));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSqlServerVectorStore(o=>connectionString);

    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapApiEndpoints();

app.Run();

