using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Models;
using mypaperwork.Models.Filter;
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
    public async Task<IActionResult> CreatePaperwork([FromBody] CreatePaperworkRequestModel createPaperworkRequestModel)
    {
        var response = await _paperworkServices.CreatePaperwork(createPaperworkRequestModel);
            
        return Transform(response);
    }
    [HttpPost("update")]
    [Authorize]
    public async Task<IActionResult> UpdatePaperwork([FromBody] UpdatePaperworkRequestModel updatePaperworkRequestModel)
    {
        var response = await _paperworkServices.UpdatePaperwork(updatePaperworkRequestModel);
            
        return Transform(response);
    }
    [HttpDelete("delete/category/{categoryGUID:length(36)}/paperwork/{paperworkGUID:length(36)}")]
    [Authorize(UserRole.Admin)]
    public async Task<IActionResult> UpdatePaperwork(string categoryGUID, string paperworkGUID)
    {
        var response = await _paperworkServices.DeletePaperwork(categoryGUID, paperworkGUID);
            
        return Transform(response);
    }
    [HttpGet("getbyfile")]
    [Authorize]
    public async Task<IActionResult> GetByFile([FromQuery] PaginationFilter filter)
    {
        var response = await _paperworkServices.GetByFile(filter);
            
        return Transform(response);
    }
    [HttpGet("getbycategory/{categoryGUID:length(36)}")]
    [Authorize]
    public async Task<IActionResult> GetByCategory(string categoryGUID, [FromQuery] PaginationFilter filter)
    {
        var response = await _paperworkServices.GetByCategory(categoryGUID, filter);
            
        return Transform(response);
    }
}