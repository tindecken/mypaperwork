namespace StockKAutoManagement_API.Middlewares;
public class EnableRequestRewindMiddleware
{
    private readonly RequestDelegate _next;

    public EnableRequestRewindMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.EnableBuffering(); // this used to be EnableRewind
        await _next(context);
    }
}
