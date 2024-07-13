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
            // if userGUID == null, user is not logged in --> return Unauthorized
            var userGUID = tokenClaims.Claims.FirstOrDefault(c => c.Type == "userGUID")?.Value;
            if (string.IsNullOrEmpty(userGUID))
            {
                responseData.Success = false;
                responseData.Message = "Unauthorized";
                responseData.StatusCode = HttpStatusCode.Unauthorized;
                context.Result = new JsonResult(responseData) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                var userRequireRoles = context.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>()
                    .SelectMany(x => x._userRoles);
                if (!userRequireRoles.Any()) return;
                // get current user sytemRole
                var systemRole = tokenClaims.Claims.FirstOrDefault(c => c.Type == "systemRole")?.Value;
                UserRole currentUserRole;
                bool isSystemRoleInRoleList = Enum.TryParse(systemRole, out currentUserRole) && userRequireRoles.Contains(currentUserRole);
                if (isSystemRoleInRoleList) return;
                // get current user selectedFileRole
                var selectedFileRole = tokenClaims.Claims.FirstOrDefault(c => c.Type == "selectedFileRole")?.Value;
                bool isSelectedFileRoleInRoleList = Enum.TryParse(selectedFileRole, out currentUserRole) && userRequireRoles.Contains(currentUserRole);
                if (isSelectedFileRoleInRoleList) return; 

                responseData.Success = false;
                responseData.Message = "Forbidden - Require role(s): " + string.Join(", ", userRequireRoles) + " but systemRole: " + systemRole + " or selectedFileRole: " + selectedFileRole;
                responseData.StatusCode = HttpStatusCode.Forbidden;
                context.Result = new JsonResult(responseData) { StatusCode = StatusCodes.Status403Forbidden };
            }
        }
    }
}