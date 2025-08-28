
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol.AspNetCore.Authentication;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(o =>
{
    o.DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme;
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o=>
{
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "at-mcp-server",
        ValidateAudience = true,
        ValidAudience ="at-audience",
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YOUR_TOP_$ECRET_Key_here_1234_4321")),
        ValidateIssuerSigningKey =true
    };

    o.Events = new JwtBearerEvents
    {
        OnTokenValidated = ctx =>
        {
            Debug.WriteLine($"OnTokenValidated => {ctx.Principal?.Identity?.Name ?? ""}");

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = ctx =>
        {
            Debug.WriteLine($"OnAutheticationFailed");
            return Task.CompletedTask;
        },
    };
}).AddMcp();

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddMcpServer().WithToolsFromAssembly().WithHttpTransport();


var app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

app.MapMcp().RequireAuthorization();

app.Run();
