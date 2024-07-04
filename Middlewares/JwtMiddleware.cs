using mypaperwork.Services.User;
using mypaperwork.Utils;

namespace mypaperwork.Middlewares;
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _JWTsecret;
    private readonly AppSettings _appSettings;
    private readonly JWTUtils _jwtUtils;

    public JwtMiddleware(RequestDelegate next, AppSettings appSettings, JWTUtils jwtUtils)
    {
        _next = next;
        _appSettings = appSettings;
        _JWTsecret = _appSettings.JWTSecret;
        _jwtUtils = jwtUtils;
    }

    public async Task Invoke(HttpContext httpContext, UserServices userService)
    {
        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            _jwtUtils.attachUserToContext(httpContext, userService, token);

        await _next(httpContext);
    }
}