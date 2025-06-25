using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

var builder = WebApplication.CreateBuilder(args);

McpClientOptions mcpOptions = new()
{
    ClientInfo = new() { Name = "WebApiClient", Version = "1.0.0" }
};

SseClientTransport ct = new(new() { Endpoint = new Uri("https://localhost:7182"), Name="SseTransport" });

builder.Services.AddTransient<Task<IMcpClient>>((sp) => {  return McpClientFactory.CreateAsync(ct, mcpOptions); });

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/f2c", async Task<IResult> (Task<IMcpClient> mcpClientTask, double temp) =>
{
Dictionary<string, object?> a = new()
{
    ["fTemp"] = temp
};

var mcpClient = await mcpClientTask;

var res = await mcpClient.CallToolAsync("ConvertF2C", a).ConfigureAwait(false);

    if (!res.IsError)
    {
        if (res.Content.Count > 0)
        {
            ContentBlock cb = res.Content[0];
            var text = ((TextContentBlock)cb).Text;

            return TypedResults.Ok(text);
        }
    }

    return TypedResults.BadRequest();
});
app.Run();


