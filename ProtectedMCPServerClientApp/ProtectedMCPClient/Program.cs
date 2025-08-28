using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

string MCPServerUrl = "https://localhost:7222/";

using HttpClient client = new HttpClient();

var clf = LoggerFactory.Create(b=>b.AddConsole());

SseClientTransport transport = new(
    new()
    {
        Endpoint = new Uri(MCPServerUrl),
        Name = "Test MCP Protected Server",
        AdditionalHeaders = new Dictionary<string, string>
        {
            // Comment this line to test the UnAuthorized use case
           ["Authorization"] = $"Bearer {CreateJWT()}"
        }
    },
     client,
     clf
    );

try
{

    await using var mcpClient = await McpClientFactory.CreateAsync(transport, null, clf).ConfigureAwait(false);

    var tools = await mcpClient.ListToolsAsync();

    foreach (var tool in tools)
    {
        Console.WriteLine($"{tool.Name} --> {tool.Description}");
    }

    Dictionary<string, object?> a = new()
    {
        ["fTemp"] = 99
    };

    var res = await mcpClient.CallToolAsync("ConvertF2C", a)
                            .ConfigureAwait(false);

    if (!res.IsError ?? true)
    {
        if (res?.Content.Count > 0)
        {
            ContentBlock cb = res.Content[0];
            var text = ((TextContentBlock)cb).Text;

            Console.WriteLine(text);
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
Console.ReadLine();
static string CreateJWT()
{
    SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes("YOUR_TOP_$ECRET_Key_here_1234_4321"));
    SigningCredentials creds= new(key,SecurityAlgorithms.HmacSha256);

    JwtSecurityToken token = new(
        issuer: "at-mcp-server",
        audience:"at-audience",
        claims: [new Claim(ClaimTypes.Name,"at-user")],
        expires:DateTime.UtcNow.AddHours(1),
        signingCredentials:creds
        );

    return new JwtSecurityTokenHandler().WriteToken(token); 
    
}
