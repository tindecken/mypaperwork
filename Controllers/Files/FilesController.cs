using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Services.File;
using mypaperwork.Utils;

namespace mypaperwork.Controllers.Files
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : TransformResponse
    {
        private readonly AppSettings _appSettings;
        private FileServices _fileServices;
        public FilesController(AppSettings appSettings, FileServices fileServices, HttpContextUtils httpContextUtils)
        {
            _appSettings = appSettings;
            _fileServices = fileServices;
        }

        [HttpGet("selectfile/{fileGUID}")]
        [Authorize]
        public async Task<IActionResult> SelectFile(string fileGUID)
        {
            var response = await _fileServices.SelectFile(fileGUID);
            
            return Transform(response);
        }
    }
}
