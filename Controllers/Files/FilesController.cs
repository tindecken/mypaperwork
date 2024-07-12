using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Services.File;
using mypaperwork.Utils;

namespace mypaperwork.Controllers.Files
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : TransformResponse
    {
        private readonly AppSettings _appSettings;
        private FilesServices _filesServices;
        public CategoriesController(AppSettings appSettings, FilesServices filesServices, HttpContextUtils httpContextUtils)
        {
            _appSettings = appSettings;
            _filesServices = filesServices;
        }

        [HttpGet("selectfile/{fileGUID}")]
        [Authorize]
        public async Task<IActionResult> SelectFile(string fileGUID)
        {
            var response = await _filesServices.SelectFile(fileGUID);
            
            return Transform(response);
        }
    }
}
