﻿using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Models;
using mypaperwork.Models.Authentication;
using mypaperwork.Services.Files;
using mypaperwork.Services.User;
using mypaperwork.Utils;

namespace mypaperwork.Controllers.Files
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : TransformResponse
    {
        private readonly AppSettings _appSettings;
        private FilesServices _filesServices;
        public FilesController(AppSettings appSettings, FilesServices filesServices, HttpContextUtils httpContextUtils)
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
