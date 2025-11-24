using ModelContextProtocol.Server;
using System.ComponentModel;

namespace WebApi;

[McpServerToolType]
public class WeatherTool
{
    private readonly List<WeatherModel> weatherData = new()
    {
        new WeatherModel { City = "New York", Temperature=22, Condition="Sunny"},
        new WeatherModel { City = "London", Temperature = 32, Condition="Hot" },
        new WeatherModel { City = "Chennai", Temperature = 33, Condition="Cloudy"}
    };
    [McpServerTool(Name ="get_weather_mcp"), Description("Gets the current weather of the specified city")]
    public async Task<WeatherModel?> GetWeatherAsyn(string city)
    {
        Console.WriteLine($"Input City:{city}");
        return weatherData.FirstOrDefault(c => c.City.Equals(city, StringComparison.OrdinalIgnoreCase));
    }
}

public class WeatherModel
{
    public string City { get; internal set;  }
    public int Temperature { get; internal set; }
    public string Condition { get; internal set; }
}
