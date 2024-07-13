using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Models.Paperwork;
using mypaperwork.Services.Paperwork;
using mypaperwork.Utils;

namespace mypaperwork.Controllers.Paperworks;

[ApiController]
[Route("[controller]")]
public class PaperworksController: TransformResponse
{
    private readonly AppSettings _appSettings;
    private PaperworkServices _paperworkServices;
    public PaperworksController(AppSettings appSettings, PaperworkServices paperworkServices, HttpContextUtils httpContextUtils)
    {
        _appSettings = appSettings;
        _paperworkServices = paperworkServices;
    }

    [HttpPost()]
    [Authorize]
    public async Task<IActionResult> SelectFile([FromBody] CreatePaperworkRequestModel createPaperworkRequestModel)
    {
        var response = await _paperworkServices.CreatePaperwork(createPaperworkRequestModel);
            
        return Transform(response);
    }
}