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

    public async Task Invoke(HttpContext context, UserServices userService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            _jwtUtils.attachUserToContext(context, userService, token);

        await _next(context);
    }
}