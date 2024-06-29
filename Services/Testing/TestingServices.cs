using System.Net;
using mypaperwork.Models;
using mypaperwork.Utils;
using mypaperwork.Models.Logging;

namespace mypaperwork.Services.TestingServices;
public class TestingServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;
    public TestingServices(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _appSettings = new AppSettings();   
    }

    public async Task<GenericResponseData> GetAppSettings()
    {
        var responseData = new GenericResponseData();
        responseData.Data = _appSettings;
        responseData.StatusCode = HttpStatusCode.OK;
        responseData.Message = "App Settings";

        return responseData;

    }
    
}