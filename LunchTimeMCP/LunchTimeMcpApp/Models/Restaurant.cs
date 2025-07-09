using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LunchTimeMcpApp.Models;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string FoodType { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; } = DateTime.Now;
}

[JsonSerializable(typeof(List<Restaurant>))]
[JsonSerializable(typeof(Restaurant))]
[JsonSerializable(typeof(RestaurantVisitInfo))]
[JsonSerializable(typeof(Dictionary<string, RestaurantVisitInfo>))]
[JsonSerializable(typeof(RestaurantData))]
[JsonSerializable(typeof(FormattedRestaurantStat))]
[JsonSerializable(typeof(FormattedRestaurantStats))]
internal sealed partial class RestaurantContext: JsonSerializerContext
{
    
}