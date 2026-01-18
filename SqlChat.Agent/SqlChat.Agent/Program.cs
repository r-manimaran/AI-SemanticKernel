using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Microsoft.SemanticKernel;
using SqlChat.Agent;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OpenAPI v1", Version = "v1" });
});

builder.Services.AddSingleton(sp =>
{
    var kernel = Kernel.CreateBuilder()
         .AddOpenAIChatCompletion(modelId:builder.Configuration["OpenAI:DeploymentName"],          
            apiKey: builder.Configuration["OpenAI:ApiKey"])
            .Build();
    kernel.ImportPluginFromObject(new SqlQueryTool(configuration: builder.Configuration,
        sp.GetRequiredService<ILogger<SqlQueryTool>>()),"sqlTool");

    return kernel;
});

builder.Services.AddScoped<ISqlChatAgent, SqlChatAgent>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenAPI v1");
    });
}
else
{
    // If you want Swagger in non-development, enable these lines:
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenAPI v1");
    });
}
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");


app.MapPost("/api/chat", async (ISqlChatAgent agent, [FromBody] ChatRequest request) =>
{
    var result = await agent.AskAsync(request.Question);
    return Results.Ok(new
    {
        answer =  result }
    );
});

app.Run();

public record ChatRequest(string Question);
internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
