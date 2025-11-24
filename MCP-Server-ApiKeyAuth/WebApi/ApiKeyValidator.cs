namespace WebApi;

public class ApiKeyValidator : IApiKeyValidator
{
    public const string APIKEY = "KEY-Xyz-1234";
    public bool IsValidApiKey(string apiKey)
    {
        if(string.IsNullOrEmpty(apiKey)) return false;

        if(apiKey == APIKEY) return true;
        
        return false;

    }
}

public interface IApiKeyValidator
{
    bool IsValidApiKey(string apiKey);
}
