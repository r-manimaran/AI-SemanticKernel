namespace WebApi.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UserQuery
{
    public string Query { get; set; } = string.Empty;
}