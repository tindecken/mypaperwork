using mypaperwork.Utils;
using mypaperwork.Models.Logging;

namespace mypaperwork.Services.Logging;
public class LoggingServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LoggingServices(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task AddLog(LogDTO logDTO)
    {
        var httpContextUtils = new HttpContextUtils(_httpContextAccessor);
        var userId = httpContextUtils.getUserId();
        var ipAddress = httpContextUtils.getClientIPAddress();
    }
}