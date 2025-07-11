using LunchTimeMcpApp.Models;
using LunchTimeMcpApp.Services;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LunchTimeMcpApp.MCPTools;
[McpServerToolType]
public sealed class RestaurantTools
{
    private readonly IRestaurantService _restaurantService;
    public RestaurantTools(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }
    [McpServerTool, Description("Get a list of all restaurants available for lunch")]
    public async Task<string> GetRestaurants()
    {
        var restaurants = await _restaurantService.GetRestaurantsAsync();
        return JsonSerializer.Serialize(restaurants, RestaurantContext.Default.ListRestaurant);
    }


    [McpServerTool, Description("Pick a random restaurant for lunch")]
    public async Task<string> PickRandomRestaurant()
    {
        var restaurant = await _restaurantService.PickRandomRestaurantAsync();
        if (restaurant == null)
        {
            return JsonSerializer.Serialize(new { message = "No restaurants available. Please add some to begin." });
        }
        return JsonSerializer.Serialize(new
        {
            message = $"🍽️ Time for lunch at {restaurant.Name}!",
            restaurant = restaurant
        });
    }

    [McpServerTool, Description("Add a new restaurant to the lunch options")]
    public async Task<string> AddRestaurant(
        [Description("The name of the restaurant")]string name, 
        [Description("The location/address of the restaurant")]string address,
        [Description("The type of food served (e.g., Indian, Thai, Mexican")]string foodType)
    {
        var restaurant = await _restaurantService.AddRestaurantAsync(name, address, foodType);
        return JsonSerializer.Serialize(restaurant, RestaurantContext.Default.Restaurant);
    }

    [McpServerTool, Description("Remove a restaurant from the lunch options")]
    public async Task<string> RemoveRestaurant(
        [Description("The ID of the restaurant to remove")]int id)
    {
        var success = await _restaurantService.RemoveRestaurantAsync(id);
        if (success)
        {
            return JsonSerializer.Serialize(new { message = "Restaurant removed successfully." });
        }
        return JsonSerializer.Serialize(new { message = "Restaurant not found." });
    }

    [McpServerTool, Description("Get the statistics about how many times each restaurant has been visited.")]
    public async Task<string> GetVisitStatistics()
    {
        var formattedStats = await _restaurantService.GetFormattedVisitStatsAsync();
        return JsonSerializer.Serialize(formattedStats, RestaurantContext.Default.FormattedRestaurantStats);
    }
}
