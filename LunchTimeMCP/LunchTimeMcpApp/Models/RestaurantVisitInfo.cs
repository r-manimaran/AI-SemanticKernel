namespace LunchTimeMcpApp.Models;

public class RestaurantVisitInfo
{
    public Restaurant Restaurant { get; set; } = new();
    public DateTime? LastVisited { get; set; }
    public int VisitCount { get; set; } 
}
