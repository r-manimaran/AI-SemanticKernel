using Microsoft.Extensions.AI;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OllamaSharp;
using WebApi;
using WebApi.Appsettings;
using WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// builder.Services.AddAzureOpenAIEmbeddingGenerator(deploymentName: "text-embedding-3-small", apiKey: builder.Configuration["AzureOpenAI:ApiKey"], endpoint: builder.Configuration["AzureOpenAI:Endpoint"]);

builder.Services.Configure<AISettings>(builder.Configuration.GetSection(AISettings.SectionName));

var aiSettings = builder.Configuration.GetSection(AISettings.SectionName).Get<AISettings>();

builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp => new OllamaApiClient(aiSettings.OllamaEndpoint, aiSettings.EmbeddingModel));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSqlServerVectorStore(o=>connectionString);

builder.Services.AddHealthChecks()
    .AddCheck<CheckSqlConnection>("SQL Database");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
   ResponseWriter = async (context,report) => {
       
           context.Response.ContentType = "application/json";

           var response = new
           {
               status = report.Status.ToString(),
               results = report.Entries.Select(e => new
               {
                   component = e.Key,
                   status = e.Value.Status.ToString(),
                   description = e.Value.Description,
                   duration = e.Value.Duration.ToString()
               }),
               TototalDuration = report.TotalDuration
           };
           await context.Response.WriteAsJsonAsync(response);
       }
});

app.UseHttpsRedirection();

app.MapApiEndpoints();

app.Run();

