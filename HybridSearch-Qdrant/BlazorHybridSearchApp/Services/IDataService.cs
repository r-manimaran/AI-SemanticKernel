
namespace BlazorHybridSearchApp.Services
{
    public interface IDataService
    {
        Task<List<string>> GetRestaurantInfo(string query, int topK = 5);
        Task LoadData(string filePath);
    }
}