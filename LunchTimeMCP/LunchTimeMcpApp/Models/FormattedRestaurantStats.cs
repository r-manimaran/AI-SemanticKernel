namespace LunchTimeMcpApp.Models;

public class FormattedRestaurantStats
{
    public string Message { get; set; } = string.Empty;
    public List<FormattedRestaurantStat> Statistics { get; set; } = new();
    public int TotalRestaurants { get; set; }
    public int TotalVisits { get; set; }
}