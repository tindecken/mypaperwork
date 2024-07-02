using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using mypaperwork.Models;
using mypaperwork.Models.Database;

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
        var user = (Users)context.HttpContext.Items["Users"];
        UserRole currentUserRole;
        if (user == null)
        {
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
        else
        {
            bool isUserRoleInRoleList = Enum.TryParse(user.SystemRole, out currentUserRole) && _userRoles.Contains(currentUserRole);
            if (_userRoles.Any() && !isUserRoleInRoleList)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}