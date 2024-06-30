using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Models;
using mypaperwork.Services.Logging;
using mypaperwork.Services.Testing;

namespace mypaperwork.Controllers.Testing;

[ApiController]
[Route("[controller]")]
public class Testing : TransformResponse
{
    private readonly TestingServices _testingServices;
    private readonly LoggingServices _loggingServices;
    public Testing(TestingServices testingServices, LoggingServices loggingServices)
    {
        _testingServices = testingServices;
        _loggingServices = loggingServices;

    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAppSettings()
    {
        var response = await _testingServices.GetAppSettings();
        return Transform(response);
    }
    
    [AllowAnonymous]
    [HttpGet("serilog")]
    public async Task<IActionResult> SerialLog()
    {
        var response = await _testingServices.SeriLog();
        return Transform(response);
    }
    [AllowAnonymous]
    [HttpGet("logging")]
    public async Task<IActionResult> AddLog()
    {
        var response = await _loggingServices.AddLog();
        return Transform(response);
    }
    [AllowAnonymous]
    [HttpGet("seedingdatabase")]
    public async Task<IActionResult> SeedingDatabase()
    {
        var response = await _testingServices.SeedingDatabase();
        return Transform(response);
    }
}

