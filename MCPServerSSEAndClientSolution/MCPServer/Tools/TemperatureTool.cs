using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;
[McpServerToolType]
public class TemperatureTool
{
    [McpServerTool, Description("Convert Fahrenheit to Celcius")]
    public static double ConvertF2C([Description("Fahrenheit temperature to convert")] double fTemp)
    {
        return (fTemp - 32) * 5.0 / 9.0;
    }
}
