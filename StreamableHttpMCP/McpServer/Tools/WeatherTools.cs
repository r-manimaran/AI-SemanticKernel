using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpServer.Tools;
[McpServerToolType]
public static class WeatherTools
{
    [McpServerTool, Description("Get the current weather for a city")]
    public static string GetWeather([Description("City name")] string city)
    {
        return $"The weather in {city} is 22°C and sunny.";
    }


    [McpServerTool, Description("List supported cities")]
    public static string[] ListCities() =>
        ["London", "New York", "Tokyo", "Sydney"];
}
