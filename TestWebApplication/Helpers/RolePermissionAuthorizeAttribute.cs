using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using TestWebApplication.Data;

namespace TestWebApplication.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RolePermissionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private string _actionType;

        private ClaimsPrincipal _principal;

        public RolePermissionAuthorizeAttribute(string actionType)
        {
            _actionType = actionType;
            _principal = new ClaimsPrincipal();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            bool allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
            {
                return;
            }

            _principal = context.HttpContext.User;

            if (!_principal.Identity?.IsAuthenticated ?? false)
            {
                // it isn't needed to set unauthorized result
                // as the base class already requires the user to be authenticated
                // this also makes redirect to a login page work properly
                // context.Result = new UnauthorizedResult();
                return;
            }

            string? userId = _principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ApplicationDbContext dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            List<string> userRoleIds = dbContext.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();

            if (!userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
            {
                List<Models.RolePermission> rolePermissionAccess = dbContext.RolePermission.Where(t => userRoleIds.Contains(t.RoleID.ToString())).ToList();
                string controller = context.RouteData.Values["controller"] + string.Empty;

                if (!rolePermissionAccess.Any(r => r.ModuleName == controller
                && ((r.IsCreateAccess && _actionType == "Create")
                    || (r.IsReadAccess && _actionType == "Read")
                    || (r.IsUpdateAccess && _actionType == "Update")
                    || (r.IsDeleteAccess && _actionType == "Delete"))))
                {
                    //context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    //context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);

                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                    return;
                }
            }
        }
    }
}
