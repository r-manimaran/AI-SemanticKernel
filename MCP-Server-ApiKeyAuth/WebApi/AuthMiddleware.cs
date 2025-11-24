namespace WebApi;

public class AuthMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    public async Task InvokeAsync(HttpContext context, IApiKeyValidator validator)
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            await context.Response.WriteAsync("You are not authorized");
            return;
        }

        var apiKey = context.Request.Headers["Authorization"];
        if (!validator.IsValidApiKey(apiKey))
        {
            await context.Response.WriteAsync("Authentication failed");
            return;
        }
        await _next(context);
    }
}