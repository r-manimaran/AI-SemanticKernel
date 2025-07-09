namespace LunchTimeMcpApp.Models;

public class RestaurantData
{
    public List<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    public Dictionary<string, int> VisitCounts { get; set; } = new Dictionary<string, int>();
}
