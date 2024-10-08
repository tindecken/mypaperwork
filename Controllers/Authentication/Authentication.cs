﻿using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Models;
using mypaperwork.Models.Authentication;
using mypaperwork.Services.User;

namespace mypaperwork.Controllers.Authentication
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : TransformResponse
    {
        private readonly AppSettings _appSettings;
        private UserServices _userServices;
        public AuthenticationController(AppSettings appSettings, UserServices userServices)
        {
            _appSettings = appSettings;
            _userServices = userServices;
        }

        /// <summary>
        /// Login to system with user/pass
        /// </summary>
        /// <remarks>
        /// Return: Token contains role, userId, ...
        /// 
        /// Sample request:
        /// 
        ///     {
        ///         "username": "tindecken",
        ///         "password": "password"
        ///     }
        /// </remarks>
        /// <param name="model"></param>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(AuthenticateRequestModel model)
        {
            var response = await _userServices.Authenticate(model);

            return Transform(response);
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(RegisterRequestModel model)
        {
            var response = await _userServices.RegisterUser(model);

            return Transform(response);
        }
        [HttpPost("changepassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestModel model)
        {
            var response = await _userServices.ChangePassword(model);

            return Transform(response);
        }
    }
}
