using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace PluginsApp.Plugins.Weather;

public class WeatherPlugin
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public WeatherPlugin(IConfiguration configuration)
    {
         string apiKey = configuration["Weather:ApiKey"] ?? throw new InvalidOperationException("API key not found in configuration.");
        _apiKey = apiKey;
        _httpClient = new HttpClient();
    }
    [KernelFunction("get_weather")]
    [Description("Fetches current weather data for a specified city.")]
    public async Task<string> GetWeatherAsync([Description("Name of the City")]string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City name cannot be null or empty.", nameof(city));
        }
        try
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching weather data: {response.ReasonPhrase}");
            }
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to fetch weather data for {city}.", ex);
        }
    }
}
