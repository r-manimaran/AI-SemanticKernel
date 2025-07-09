using LunchTimeMcpApp.Models;

namespace LunchTimeMcpApp.Services
{
    public interface IRestaurantService
    {
        Task<Restaurant> AddRestaurantAsync(string name, string address, string foodType);
        Task<FormattedRestaurantStats> GetFormattedVisitStatsAsync();
        Task<List<Restaurant>> GetRestaurantsAsync();
        Task<Dictionary<string, RestaurantVisitInfo>> GetVisitCountsAsync();
        Task<Restaurant> PickRandomRestaurantAsync();
        Task<bool> RemoveRestaurantAsync(int id);
    }
}