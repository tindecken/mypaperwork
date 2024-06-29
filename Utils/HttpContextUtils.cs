using mypaperwork.Models.Database;

namespace mypaperwork.Utils;

public class HttpContextUtils
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public HttpContextUtils(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int getUserId()
    {
        var userId = -1;
        var user = (Users)_httpContextAccessor.HttpContext.Items["Users"];
        if (user != null) userId = user.Id;
        return userId;
    }

    public string getClientIPAddress()
    {
        var ipAddress = _httpContextAccessor.HttpContext.Request.Headers.Keys.Contains("ClientIP") ?
            _httpContextAccessor.HttpContext?.Request.Headers["ClientIP"].ToString() :
            _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4().ToString();
        return ipAddress;
    }
}