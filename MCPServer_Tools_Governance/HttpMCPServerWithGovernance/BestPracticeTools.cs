using ModelContextProtocol.Server;
using System.ComponentModel;

namespace HttpMCPServerWithGovernance;

[McpServerToolType]
public class BestPracticeTools
{
    [McpServerTool, Description("Returns a .NET best practice for the given topic.")]
    public string GetBestPractice(string topic) =>
        $"Best practice for '{topic}': Use dependency injection and Options pattern.";

    [McpServerTool, Description("Lists recommended NuGet packages for a given category.")]
    public IEnumerable<string> ListPackages(string category) =>
        category switch
        {
            "logging" => ["Serilog", "Microsoft.Extensions.Logging"],
            "resilience" => ["Polly", "Microsoft.Extensions.Http.Resilience"],
            "observability" => ["OpenTelemetry", "Azure.Monitor.OpenTelemetry.AspNetCore"],
            _ => ["No packages found for this category"]
        };
}
