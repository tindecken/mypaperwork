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

    public string GetClientIPAddress()
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
        token.userId = tokenClaims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        token.email = tokenClaims.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        token.systemRole = tokenClaims.Claims.FirstOrDefault(c => c.Type == "systemRole")?.Value;
        token.selectedFileId = tokenClaims.Claims.FirstOrDefault(c => c.Type == "selectedFileId")?.Value;
        token.selectedFileRole = tokenClaims.Claims.FirstOrDefault(c => c.Type == "selectedFileRole")?.Value;
        return token;
    }

    public string? GetUserId()
    {
        var userId = string.Empty;
        var tokenClaims = (JwtSecurityToken)_httpContextAccessor.HttpContext.Items["Token"];
        if (tokenClaims == null) return null;
        userId = tokenClaims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        return userId;
    }
    public string? GetSelectedFileId()
    {
        var fileId = string.Empty;
        var tokenClaims = (JwtSecurityToken)_httpContextAccessor.HttpContext.Items["Token"];
        if (tokenClaims == null) return null;
        fileId = tokenClaims.Claims.FirstOrDefault(c => c.Type == "selectedFileId")?.Value;
        return fileId;
    }
}