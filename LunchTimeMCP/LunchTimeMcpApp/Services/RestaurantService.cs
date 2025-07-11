using LunchTimeMcpApp.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LunchTimeMcpApp.Services;

public class RestaurantService : IRestaurantService
{
    private readonly string dataFilePath;
    private List<Restaurant> restaurants = new();
    private Dictionary<string, int> visitCounts = new();
    private readonly ILogger<RestaurantService> _logger;

    public RestaurantService(ILogger<RestaurantService> logger)
    {
        _logger = logger;
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appDir = Path.Combine(appDataPath, "LunchTimeMcpApp");
        if (!Directory.Exists(appDir))
        {
            Directory.CreateDirectory(appDir);
        }

        dataFilePath = Path.Combine(appDir, "restaurants.json");
        LoadRestaurants();

        if (restaurants.Count == 0)
        {
            InitializeDefaultRestaurants();
            SaveData();
        }
    }

    private void SaveData()
    {
        try
        {
            var data = new RestaurantData
            {
                Restaurants = restaurants,
                VisitCounts = visitCounts
            };

            var json = JsonSerializer.Serialize(data, RestaurantContext.Default.RestaurantData);
            File.WriteAllText(dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving restaurants: {ex.Message}");
        }
    }

    private void LoadRestaurants()
    {
        if (!File.Exists(dataFilePath))
        {
            return;
        }

        try
        {
            _logger.LogInformation("Loading restaurants from {DataFilePath}", dataFilePath);
            var json = File.ReadAllText(dataFilePath);
            var loadedRestaurants = JsonSerializer.Deserialize(json, RestaurantContext.Default.RestaurantData);

            if (loadedRestaurants != null)
            {
                restaurants = loadedRestaurants.Restaurants ?? new List<Restaurant>();

                visitCounts = loadedRestaurants.VisitCounts ?? new Dictionary<string, int>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading restaurants: {ex.Message}");
            _logger.LogError(ex, "Failed to load restaurants from {DataFilePath}", dataFilePath);
        }
    }

    private void InitializeDefaultRestaurants()
    {
        var trendyRestaurants = new List<Restaurant>
        {
            // Raleigh
            new() { Id = 1, Name = "Curry Leaf Indian Restaurant", Address = "1001 Oberlin Rd, Raleigh, NC", FoodType = "Indian", DateAdded = DateTime.UtcNow },
            new() { Id = 2, Name = "Tamarind Indian Cuisine", Address = "3011 Hillsborough St, Raleigh, NC", FoodType = "Indian", DateAdded = DateTime.UtcNow },
            new() { Id = 3, Name = "Chola Indian Cuisine", Address = "1001 Wade Ave, Raleigh, NC", FoodType = "Indian", DateAdded = DateTime.UtcNow },
            new() { Id = 4, Name = "Sitar Indian Cuisine", Address = "1001 Wake Forest Rd, Raleigh, NC", FoodType = "Indian", DateAdded = DateTime.UtcNow },
            new() { Id = 5, Name = "Biryani Pot", Address = "123 Main St, Durham, NC", FoodType = "Indian", DateAdded = DateTime.UtcNow },
            new() { Id = 6, Name = "Taste of India", Address = "456 Elm St, Cary, NC", FoodType = "Indian", DateAdded = DateTime.UtcNow },
            // Morrisville
            new() { Id = 7, Name = "Swagat Indian Cuisine", Address = "9549 Chapel Hill Rd, Morrisville, NC 27560", FoodType = "Indian", DateAdded = DateTime.UtcNow }, // [2]
            new() { Id = 8, Name = "BLR Bar & Curry", Address = "10970 Chapel Hill Rd, Morrisville, NC 27560", FoodType = "Indian, Vegetarian", DateAdded = DateTime.UtcNow }, // [3]
            new() { Id = 9, Name = "Triangles Biryani Indian Restaurant", Address = "4121 Davis Dr, Morrisville, NC 27560", FoodType = "Indian", DateAdded = DateTime.UtcNow }, // [5]
            new() { Id = 10, Name = "Naga's South Indian Cuisine", Address = "1000 Lower Shiloh Way, Morrisville, NC 27560", FoodType = "South Indian", DateAdded = DateTime.UtcNow }, // [13]
            new() { Id = 11, Name = "JP's Biryani", Address = "9625 Chapel Hill Rd, Morrisville, NC 27560", FoodType = "Indian", DateAdded = DateTime.UtcNow }, // [17]

            // Cary
            new() { Id = 12, Name = "Royale Curry", Address = "990 High House Rd, Cary, NC 27513", FoodType = "Indian", DateAdded = DateTime.UtcNow }, // [6]
            new() { Id = 13, Name = "Cilantro Indian Café", Address = "9424 Chapel Hill Rd, Cary, NC 27513", FoodType = "Indian", DateAdded = DateTime.UtcNow }, // [10]
            new() { Id = 14, Name = "Saffron Indian Cuisine", Address = "1135 Kildaire Farm Rd, Cary, NC 27511", FoodType = "Indian", DateAdded = DateTime.UtcNow }, // [18]

            // Apex
            new() { Id = 15, Name = "Naan House Indian Bistro", Address = "5460 Apex Peakway, Apex, NC 27502", FoodType = "Indian", DateAdded = DateTime.UtcNow }, // [7]
            new() { Id = 16, Name = "Bawarchi Grill & Spirits", Address = "800 W Williams St #176, Apex, NC 27502", FoodType = "Indian", DateAdded = DateTime.UtcNow }, // [19]

            // Garner
            new() { Id = 17, Name = "Chhote's Indian Street Food", Address = "1155 Timber Dr E, Garner, NC 27529", FoodType = "Indian", DateAdded = DateTime.UtcNow } // [8][20]
        };
        restaurants.AddRange(trendyRestaurants);
    }

    // Operations methods
    public async Task<List<Restaurant>> GetRestaurantsAsync()
    {
        _logger.LogInformation("Retrieving all restaurants");
        return await Task.FromResult(restaurants.ToList());
    }

    public async Task<Restaurant> AddRestaurantAsync(string name, string address, string foodType)
    {
        var newRestaurant = new Restaurant
        {
            Id = restaurants.Count > 0 ? restaurants.Max(r => r.Id) + 1 : 1,
            Name = name,
            Address = address,
            FoodType = foodType,
            DateAdded = DateTime.UtcNow
        };
        restaurants.Add(newRestaurant);
        SaveData();
        return await Task.FromResult(newRestaurant);
    }

    public async Task<bool> RemoveRestaurantAsync(int id)
    {
        var restaurant = restaurants.FirstOrDefault(r => r.Id == id);
        if (restaurant != null)
        {
            restaurants.Remove(restaurant);
            SaveData();
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

    public async Task<Restaurant> PickRandomRestaurantAsync()
    {
        if (restaurants.Count == 0)
        {
            return null;
        }
        var random = new Random();
        var index = random.Next(restaurants.Count);
        var selectedRestaurant = restaurants[index];

        // Increment visit count
        if (visitCounts.ContainsKey(selectedRestaurant.Id.ToString()))
        {
            visitCounts[selectedRestaurant.Id.ToString()]++;
            _logger.LogInformation("Incremented visit count for restaurant {RestaurantId}", selectedRestaurant.Id);
        }
        else
        {
            visitCounts[selectedRestaurant.Id.ToString()] = 1;
            _logger.LogInformation("Initialized visit count for restaurant {RestaurantId}", selectedRestaurant.Id);
        }
        SaveData();
        return await Task.FromResult(selectedRestaurant);
    }

    public async Task<Dictionary<string, RestaurantVisitInfo>> GetVisitCountsAsync()
    {
        var stats = new Dictionary<string, RestaurantVisitInfo>();

        foreach (var restaurant in restaurants)
        {
            var visitCount = visitCounts.GetValueOrDefault(restaurant.Id.ToString(), 0);
            stats[restaurant.Name] = new RestaurantVisitInfo
            {
                Restaurant = restaurant,
                VisitCount = visitCount,
                LastVisited = visitCount > 0 ? DateTime.UtcNow : null
            };
        }
        return await Task.FromResult(stats);
    }

    public async Task<FormattedRestaurantStats> GetFormattedVisitStatsAsync()
    {
        var visitStats = await GetVisitCountsAsync();

        var formattedStats = visitStats.Values
            .OrderByDescending(v => v.VisitCount)
            .Select(stat => new FormattedRestaurantStat
            {
                Restaurant = stat.Restaurant.Name,
                Address = stat.Restaurant.Address,
                FoodType = stat.Restaurant.FoodType,
                VisitCount = stat.VisitCount,
                TimesEaten = stat.VisitCount == 0 ? "Never" :
                              stat.VisitCount == 1 ? "Once" :
                                $"{stat.VisitCount} times"
            }).ToList();
        return new FormattedRestaurantStats
        {
            Message = "Here are your restaurant visit statistics:",
            Statistics = formattedStats,
            TotalRestaurants = restaurants.Count,
            TotalVisits = visitStats.Values.Sum(v => v.VisitCount)
        };
    }
}
