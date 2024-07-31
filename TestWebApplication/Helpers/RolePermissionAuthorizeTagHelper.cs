using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis;
using System.Security.Claims;
using System.Security.Principal;
using TestWebApplication.Data;

namespace TestWebApplication.Helpers
{
    [HtmlTargetElement("*", Attributes = "asp-authorize-controller,asp-authorize-action")]
    public class RolePermissionAuthorizeTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-authorize-controller")]
        public string ControllerName { get; set; } = string.Empty;

        [HtmlAttributeName("asp-authorize-action")]
        public string ActionType { get; set; } = string.Empty;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public RolePermissionAuthorizeTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
            {
                ClaimsPrincipal _principal = _httpContextAccessor.HttpContext.User;

                string? userId = _principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                ApplicationDbContext dbContext = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

                List<string> userRoleIds = dbContext.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();

                if (!userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    //List<string> userRoleIds = dbContext.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();

                    List<Models.RolePermission> rolePermissionAccess = dbContext.RolePermission.Where(t => userRoleIds.Contains(t.RoleID + string.Empty)).ToList();

                    if (!rolePermissionAccess.Any(r => r.ModuleName == ControllerName
                    && ((r.IsCreateAccess && ActionType == "Create")
                        || (r.IsReadAccess && ActionType == "Read")
                        || (r.IsUpdateAccess && ActionType == "Update")
                        || (r.IsDeleteAccess && ActionType == "Delete"))))
                    {
                        output.SuppressOutput();
                    }
                }
            }

            //if (output.Attributes.TryGetAttribute("href", out var tagHelperAttribute))
            //{
            //    output.Attributes.Remove(tagHelperAttribute);
            //}
        }
    }
}
