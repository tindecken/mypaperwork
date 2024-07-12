using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Services.Category;
using mypaperwork.Services.User;
using mypaperwork.Utils;

namespace mypaperwork.Controllers.Categories
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : TransformResponse
    {
        private readonly AppSettings _appSettings;
        private CategoryServices _categoryServices;
        public CategoriesController(AppSettings appSettings, CategoryServices categoryServices, HttpContextUtils httpContextUtils)
        {
            _appSettings = appSettings;
            _categoryServices = categoryServices;
        }

        [HttpGet("selectfile/{fileGUID}")]
        [Authorize]
        public async Task<IActionResult> SelectFile([FromBody] CreateReleaseRequestModel model)
        {
            var response = await _categoryServices.CreateNewCategory   
            
            return Transform(response);
        }
    }
}
