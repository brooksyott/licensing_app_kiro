using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace LicenseManagementApi.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _allowedRoles;

    public RequireRoleAttribute(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Get role from HttpContext items (set by authentication middleware)
        if (!context.HttpContext.Items.TryGetValue("Role", out var roleObj) || roleObj == null)
        {
            context.Result = new JsonResult(new
            {
                message = "Unauthorized access",
                errorCode = "UNAUTHORIZED"
            })
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
            return;
        }

        var userRole = roleObj.ToString();

        // Check if user's role is in the allowed roles
        if (!_allowedRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = new JsonResult(new
            {
                message = "Insufficient permissions to access this resource",
                errorCode = "FORBIDDEN"
            })
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }
    }
}
