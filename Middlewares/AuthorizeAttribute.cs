using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using mypaperwork.Models;
using mypaperwork.Models.Database;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

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
        var responseData = new GenericResponseData();
        // skip authorization if action has an [AllowAnonymous] attribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous) return;
        
        // authorize based on user role
        var tokenClaims = (JwtSecurityToken)context.HttpContext.Items["Token"];
        if (tokenClaims == null)
        {
            responseData.Success = false;
            responseData.Message = "Invalid token or expired";
            responseData.StatusCode = HttpStatusCode.Unauthorized;
            context.Result = new JsonResult(responseData) { StatusCode = StatusCodes.Status401Unauthorized };
        }
        else
        {
            var userGUID = tokenClaims.Claims.FirstOrDefault(c => c.Type == "userGUID")?.Value;
            UserRole currentUserRole;
            if (string.IsNullOrEmpty(userGUID))
            {
                responseData.Success = false;
                responseData.Message = "Unauthorized";
                responseData.StatusCode = HttpStatusCode.Unauthorized;
                context.Result = new JsonResult(responseData) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                // check user role
                var systemRole = tokenClaims.Claims.FirstOrDefault(c => c.Type == "systemRole")?.Value;
                bool isUserRoleInRoleList = Enum.TryParse(systemRole, out currentUserRole) && _userRoles.Contains(currentUserRole);
                var a = _userRoles.Any();
                if (_userRoles.Any() && !isUserRoleInRoleList)
                {
                    responseData.Success = false;
                    responseData.Message = "Forbidden";
                    responseData.StatusCode = HttpStatusCode.Forbidden;
                    context.Result = new JsonResult(responseData) { StatusCode = StatusCodes.Status403Forbidden };
                }
            }
        }
    }
}