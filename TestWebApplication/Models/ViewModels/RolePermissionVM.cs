using Microsoft.AspNetCore.Identity;

namespace TestWebApplication.Models.ViewModels
{
    public class RolePermissionVM
    {
        public Guid? RoleId { get; set; }

        public List<Roles>? Roles { get; set; }

        public List<ControllerObject>? ControllerObjects { get; set; }

        public List<RolePermission>? RolePermissions { get; set; }
    }
}
