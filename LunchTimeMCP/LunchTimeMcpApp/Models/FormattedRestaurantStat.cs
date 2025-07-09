namespace LunchTimeMcpApp.Models;

public class  FormattedRestaurantStat 
{
    public string Restaurant { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string FoodType { get; set; } = string.Empty;
    public int VisitCount { get; set; } = 0;
    public string TimesEaten { get; set; } = string.Empty;
}
