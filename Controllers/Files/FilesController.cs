using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Models;
using mypaperwork.Models.Files;
using mypaperwork.Models.Filter;
using mypaperwork.Services.FileServices;
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

        [HttpGet("selectfile/{fileId}")]
        [Authorize]
        public async Task<IActionResult> SelectFile(string fileId)
        {
            var response = await _fileServices.SelectFile(fileId);
            
            return Transform(response);
        }
        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> CreateFile([FromBody] CreateFileRequestModel model)
        {
            var response = await _fileServices.CreateFile(model);
            
            return Transform(response);
        }
        [HttpDelete("{fileId:length(26)}")]
        [Authorize(UserRole.Admin)]
        public async Task<IActionResult> CreateFile(string fileId)
        {
            var response = await _fileServices.DeleteFile(fileId);
            
            return Transform(response);
        }
        [HttpGet("getfilesbyuser")]
        [Authorize]
        public async Task<IActionResult> GetFilesByUser()
        {
            var response = await _fileServices.GetFilesByUser();
            return Transform(response);
        }
    }
}
