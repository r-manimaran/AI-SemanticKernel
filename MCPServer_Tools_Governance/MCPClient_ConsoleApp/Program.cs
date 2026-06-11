using ModelContextProtocol.Client;

var transport = new HttpClientTransport(new HttpClientTransportOptions()
{
    Endpoint = new Uri("https://localhost:7177"),
    TransportMode = HttpTransportMode.StreamableHttp
});

await using var mcpClient = await McpClient.CreateAsync(transport);

var mcpTools = await mcpClient.ListToolsAsync();
Console.WriteLine("Tools available from MCP");
foreach (var tool in mcpTools)
{
    Console.WriteLine($"{tool.Name} --> {tool.Description}");
}
Console.ReadLine();