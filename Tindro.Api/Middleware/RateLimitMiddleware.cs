using Microsoft.Extensions.Caching.Memory;

public class RateLimitMiddleware
{
    private static readonly Dictionary<string, int> Requests = new();
    private readonly RequestDelegate _next;

    public RateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress!.ToString();

        if (!Requests.ContainsKey(ip))
            Requests[ip] = 0;

        Requests[ip]++;

        if (Requests[ip] > 100) // 100 requests per minute
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync("Too many requests");
            return;
        }

        await _next(context);
    }
}
