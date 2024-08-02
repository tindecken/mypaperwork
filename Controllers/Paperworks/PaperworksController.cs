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
    [HttpDelete("delete/category/{categoryId:length(26)}/paperwork/{paperworkId:length(26)}")]
    [Authorize(UserRole.Admin)]
    public async Task<IActionResult> UpdatePaperwork(string categoryId, string paperworkId)
    {
        var response = await _paperworkServices.DeletePaperwork(categoryId, paperworkId);
            
        return Transform(response);
    }
    [HttpGet("getbyfile")]
    [Authorize]
    public async Task<IActionResult> GetByFile([FromQuery] PaginationFilter filter)
    {
        var response = await _paperworkServices.GetByFile(filter);
            
        return Transform(response);
    }
    [HttpGet("getbycategory/{categoryId:length(26)}")]
    [Authorize]
    public async Task<IActionResult> GetByCategory(string categoryId, [FromQuery] PaginationFilter filter)
    {
        var response = await _paperworkServices.GetByCategory(categoryId, filter);
            
        return Transform(response);
    }
}