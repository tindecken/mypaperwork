using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Models.Categories;
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

        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestModel categoryRequestModel)
        {
            var response = await _categoryServices.CreateNewCategory(categoryRequestModel); 
            
            return Transform(response);
        }
        [HttpPost("update")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryRequestModel categoryRequestModel)
        {
            var response = await _categoryServices.UpdateCategory(categoryRequestModel);

            return Transform(response);
        }
    }
}
