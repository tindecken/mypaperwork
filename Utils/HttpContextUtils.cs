using Microsoft.AspNetCore.Authentication;
using mypaperwork.Models;
using mypaperwork.Models.Database;
using System.IdentityModel.Tokens.Jwt;

namespace mypaperwork.Utils;

public class HttpContextUtils
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public HttpContextUtils(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string getUserGUID()
    {
        var userGUID = string.Empty;
        var tokenClaims = (JwtSecurityToken)_httpContextAccessor.HttpContext.Items["Token"];
        if (tokenClaims != null)
        {
            userGUID = tokenClaims.Claims.FirstOrDefault(c => c.Type == "userGUID")?.Value;
        }

        return userGUID;
    }

    public string getClientIPAddress()
    {
        var ipAddress = _httpContextAccessor.HttpContext.Request.Headers.Keys.Contains("ClientIP") ?
            _httpContextAccessor.HttpContext?.Request.Headers["ClientIP"].ToString() :
            _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4().ToString();
        return ipAddress;
    }
    public Token GetToken()
    {
        var token = new Token();
        var tokenClaims = (JwtSecurityToken)_httpContextAccessor.HttpContext.Items["Token"];
        token.userGUID = tokenClaims.Claims.FirstOrDefault(c => c.Type == "userGUID")?.Value;
        token.email = tokenClaims.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        token.systemRole = tokenClaims.Claims.FirstOrDefault(c => c.Type == "systemRole")?.Value;
        token.selectedFileGUID = tokenClaims.Claims.FirstOrDefault(c => c.Type == "selectedFileGUID")?.Value;
        token.selectedFileRole = tokenClaims.Claims.FirstOrDefault(c => c.Type == "selectedFileRole")?.Value;
        return token;
    }
}