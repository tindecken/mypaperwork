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

    public async Task GetAppSettings()
    {
        var responseData = new GenericResponseData();
        var appSettings = new AppSettings();
        responseData.Success = true;
        responseData.StatusCode = HttpStatusCode.OK;
        responseData.Result = appSettings;
        await HttpContextUtils.ErrorLogging(_httpContextAccessor, responseData);
    }
    
}