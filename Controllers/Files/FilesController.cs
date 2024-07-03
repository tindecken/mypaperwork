using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Models.Authentication;
using mypaperwork.Services.Files;
using mypaperwork.Services.User;

namespace mypaperwork.Controllers.Files
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : TransformResponse
    {
        private readonly AppSettings _appSettings;
        private FilesServices _filesServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FilesController(AppSettings appSettings, FilesServices filesServices, IHttpContextAccessor httpContextAccessor)
        {
            _appSettings = appSettings;
            _filesServices = filesServices;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("selectfile/{fileGUID}")]
        public async Task<IActionResult> SelectFile(string fileGUID)
        {
            var response = await _filesServices.SelectFile(fileGUID);

            return Transform(response);
        }
    }
}
