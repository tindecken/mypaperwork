using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using mypaperwork.Models;
using mypaperwork.Models.Database;
using System.IdentityModel.Tokens.Jwt;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly IList<UserRole> _userRoles;
    public AuthorizeAttribute(params UserRole[] userRoles)
    {
        _userRoles = userRoles ?? new UserRole[] { };
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // skip authorization if action has an [AllowAnonymous] attribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous) return;
        
        // authorize based on user role
        
        var tokenClaims = (JwtSecurityToken)context.HttpContext.Items["Token"];
        var userGUID = tokenClaims.Claims.FirstOrDefault(c => c.Type == "userGUID")?.Value;
        if (string.IsNullOrEmpty(userGUID)) {
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        } else { 
            // check token is expired or not
            var tokenExp = tokenClaims.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tokenExp)).UtcDateTime;
            var now = DateTime.Now.ToUniversalTime();
            var isExpired =  now > tokenDate;
            if (isExpired)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            } else { 
                UserRole currentUserRole;
                var systemRole = tokenClaims.Claims.FirstOrDefault(c => c.Type == "systemRole")?.Value;
                if (string.IsNullOrEmpty(systemRole))
                {
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else
                {
                    bool isUserRoleInRoleList = Enum.TryParse(systemRole, out currentUserRole) && _userRoles.Contains(currentUserRole);
                    if (_userRoles.Any() && !isUserRoleInRoleList)
                    {
                        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    }
                }
            }
        }
        
    }
}