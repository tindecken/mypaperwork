using StockKAutoManagement_API.AppSettings;
using StockKAutoManagement_API.Services.User;
using StockKAutoManagement_API.Utils;

namespace StockKAutoManagement_API.Middlewares;
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _secret;
    private readonly AppSetting _appSetting;
    private readonly JWTUtils _jwtUtils;

    public JwtMiddleware(RequestDelegate next, AppSetting appSetting, JWTUtils jwtUtils)
    {
        _next = next;
        _appSetting = appSetting;
        _secret = _appSetting.Secret;
        _jwtUtils = jwtUtils;
    }

    public async Task Invoke(HttpContext context, UserService userService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            _jwtUtils.attachUserToContext(context, userService, token);

        await _next(context);
    }
}