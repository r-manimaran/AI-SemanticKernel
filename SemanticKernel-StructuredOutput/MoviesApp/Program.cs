using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using MoviesApp;
using System.Runtime.CompilerServices;
using System.Text.Json;


var configuration = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory)           
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddUserSecrets<Program>() // Ensure you have user secrets set up for OpenAI API key           
           .Build();


var llm = configuration["llm"] ?? "OpenAI"; // Default to OpenAI if not specified

if (llm == "AzureOpenAI")
{
    Console.WriteLine("Using Azure OpenAI");
}
else
{
    Console.WriteLine("Using OpenAI");
}

Kernel kernel = GetKernel(llm, configuration);


// Specify response format by setting Type object in prompt execution settings
var executionSettings = new OpenAIPromptExecutionSettings
{
    ResponseFormat = typeof(MovieResult)
};

// AzureOpen AI
var executionSettingsAzure = new AzureOpenAIPromptExecutionSettings
{
    ResponseFormat = typeof(MovieResult)
};

object result = string.Empty;
if (llm == "AzureOpenAI")
{
    Console.WriteLine("Using Azure OpenAI");
    Console.WriteLine("Question: What are the top 10 tamil movies of all time?");
    result = await kernel.InvokePromptAsync("What are the top 10 tamil movies of all time?", new(executionSettingsAzure));
}
else
{
    Console.WriteLine("Using OpenAI");
    Console.WriteLine("Question: What are the top 10 tamil movies of all time?");
    result = await kernel.InvokePromptAsync("What are the top 10 tamil movies of all time?", new(executionSettings));
}


// Deserialize string response to a strong type to access type properties
var movieResult = JsonSerializer.Deserialize<MovieResult>(result.ToString());

for (var i = 0; i < movieResult.Movies.Count; i++)
{
    var movie = movieResult.Movies[i];
    Console.WriteLine($"Movie #{i + 1}");
    Console.WriteLine($"Title: {movie.Title}");
    Console.WriteLine($"Director: {movie.Director}");
    Console.WriteLine($"Music Composer: {movie.MusicComposer}");
    Console.WriteLine($"Release year: {movie.ReleaseYear}");
    Console.WriteLine($"Rating: {movie.Rating}");
    Console.WriteLine($"Is available on streaming: {movie.IsAvailableOnStreaming}");
    Console.WriteLine($"Tags: {string.Join(",", movie.Tags)}");
    Console.WriteLine("---------------------------------------------------");
}


Kernel GetKernel(string llm, IConfiguration configuration)
{
    if (llm == "AzureOpenAI")
    {
      
        return Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: configuration["AzureOpenAI:DeploymentNameModelId"],
                endpoint: configuration["AzureOpenAI:Endpoint"],
                apiKey: configuration["AzureOpenAI:ApiKey"])
            .Build();
    }
    else
    {
        var apiKey = Environment.GetEnvironmentVariable("OpenAI__ApiKey") ?? configuration["OpenAI:ApiKey"];

        return Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(
                modelId: configuration["OpenAI:ModleId"] ?? "",
                apiKey: apiKey)
            .Build();
    }
}

