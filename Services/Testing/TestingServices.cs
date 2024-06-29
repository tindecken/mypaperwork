using System.Net;
using mypaperwork.Models;
using mypaperwork.Services.Logging;
using Serilog;

namespace mypaperwork.Services.Testing;
public class TestingServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;
    private readonly LoggingServices _loggingServices;
    public TestingServices(IHttpContextAccessor httpContextAccessor, AppSettings appSettings, LoggingServices loggingServices)
    {
        _httpContextAccessor = httpContextAccessor;
        _appSettings = appSettings;
        _loggingServices = loggingServices;
    }

    public async Task<GenericResponseData> GetAppSettings()
    {
        var responseData = new GenericResponseData
        {
            Success = true,
            Data = _appSettings,
            StatusCode = HttpStatusCode.OK,
            Message = "App Settings"
        };

        return responseData;
    }
    public async Task<GenericResponseData> SeriLog()
    {
        Log.Information("Hello world");
        var responseData = new GenericResponseData
        {
            Success = true,
            Data = _appSettings,
            StatusCode = HttpStatusCode.OK,
            Message = "Check the log file"
        };
        return responseData;
    }
}