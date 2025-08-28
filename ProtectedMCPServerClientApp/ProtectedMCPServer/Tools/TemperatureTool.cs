using ModelContextProtocol.Server;
using System.ComponentModel;

namespace ProtectedMCPServer.Tools;


[McpServerToolType]
public class TemperatureTool
{
    [McpServerTool(Name ="ConvertF2C"), Description("Convert Fahrenheit to Celcius")]
    public static double ConvertF2C([Description("Fahrenheit to celcius")] double fTemp)
    {
        return (fTemp - 32) * 5.0 / 9.0;
    }
}
