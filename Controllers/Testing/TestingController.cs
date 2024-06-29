using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Services.TestingServices;

namespace mypaperwork.Controllers.Testing;

[ApiController]
[Route("[controller]")]
public class Testing : TransformResponse
{
    private readonly TestingServices _testingServices;

    public Testing(TestingServices testingServices)
    {
        _testingServices = testingServices;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAppSettings()
    {
        var response = await _testingServices.GetAppSettings();
        return Transform(response);
    }

}

