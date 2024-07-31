using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class RolePermissionController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration _Configuration;

        public RolePermissionController(ApplicationDbContext context, IConfiguration configuration)
        {
            this.context = context;
            _Configuration = configuration;

        }
        // GET: RolePermissionController

        [RolePermissionAuthorize("Read")]
        public ActionResult Index()
        {
            RolePermissionVM vm = new()
            {
                Roles = context.Roles.Where(t => t.RoleType != "SUPERADMIN" && t.IsActive == true).ToList()
            };

            return View(vm);
        }

        // POST: RolePermissionController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RolePermissionVM rolePermissionVM)
        {
            try
            {
                if (rolePermissionVM == null)
                {
                    ModelState.AddModelError("", "Post parameter is empty.");
                    rolePermissionVM = new RolePermissionVM();
                    rolePermissionVM.Roles = context.Roles.Where(t => t.RoleType != "SuperAdmin").ToList();
                    return View(rolePermissionVM);
                }

                if (ModelState.IsValid)
                {
                    List<RolePermission> removeRolePermissionList = context.RolePermission
                        .Where(r => r.RoleID == rolePermissionVM.RoleId).ToList();

                    if (rolePermissionVM.RolePermissions == null)
                    {
                        rolePermissionVM.RolePermissions = new List<RolePermission>();
                    }

                    removeRolePermissionList = removeRolePermissionList.FindAll(r => !rolePermissionVM.RolePermissions.Any(v => v.ModuleName == r.ModuleName));

                    if (removeRolePermissionList != null && removeRolePermissionList.Count > 0)
                    {
                        context.RolePermission.RemoveRange(removeRolePermissionList);
                    }

                    DateTime actionDate = DateTime.Now;
                    rolePermissionVM.RolePermissions?.ForEach(r =>
                    {
                        if (r.RolePermissionID == 0)
                        {
                            r.RoleID = rolePermissionVM.RoleId ?? Guid.NewGuid();
                            r.CreatedOn = actionDate;
                            r.CreatedBy = User.Identity?.Name ?? string.Empty;
                            r.ModifiedOn = actionDate;
                            r.ModifiedBy = User.Identity?.Name ?? string.Empty;
                            context.RolePermission.Add(r);
                        }
                        else
                        {
                            RolePermission? existing = context.RolePermission.Local.SingleOrDefault(o => o.RolePermissionID == r.RolePermissionID);
                            if (existing != null)
                            {
                                context.Entry(existing).State = EntityState.Detached;
                            }

                            r.ModifiedOn = actionDate;
                            r.ModifiedBy = User.Identity?.Name ?? string.Empty;
                            context.RolePermission.Update(r);
                        }
                    });

                    context.SaveChanges();

                    rolePermissionVM = GetRolePermissionVM(string.Empty, rolePermissionVM.RoleId);
                    rolePermissionVM.Roles = context.Roles.ToList();

                    return View(rolePermissionVM);
                }
                else
                {
                    ModelState.AddModelError("", "Model Invalid");
                    rolePermissionVM.Roles = context.Roles.ToList();
                    return View(rolePermissionVM);

                }
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult LoadListDataPartial(string sortOrder, Guid roleId)
        {
            try
            {
                RolePermissionVM rolePermissionVM = GetRolePermissionVM(sortOrder, roleId);

                if (sortOrder.ToLower(CultureInfo.InvariantCulture) == "descending")
                {
                    return PartialView("_ListDataPartial", rolePermissionVM);
                }
                else
                {
                    return PartialView("_ListDataPartial", rolePermissionVM);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private RolePermissionVM GetRolePermissionVM(string sortOrder, Guid? roleId)
        {
            if (roleId == Guid.Empty)
            {
                return new RolePermissionVM { RolePermissions = new List<RolePermission>() };
            }

            List<RolePermission> rolePermissionList = new List<RolePermission>();

            ControllerObject[] controllerArray = _Configuration.GetSection("ControllerArray").Get<ControllerObject[]>();

            if (controllerArray != null && controllerArray.Length > 0)
            {

                controllerArray.ToList().ForEach((c) => { rolePermissionList.Add(new RolePermission() { ModuleName = c.Name }); });
            }

            List<RolePermission> dbRolePermissions = context.RolePermission.AsNoTracking().Where(m => m.RoleID == roleId).ToList();

            foreach (RolePermission item in dbRolePermissions)
            {
                RolePermission? existingItem = rolePermissionList.FirstOrDefault(r => r.ModuleName.Equals(item.ModuleName, StringComparison.OrdinalIgnoreCase));
                if (existingItem != null)
                {
                    existingItem.RolePermissionID = item.RolePermissionID;
                    existingItem.RoleID = item.RoleID;
                    existingItem.IsCreateAccess = item.IsCreateAccess;
                    existingItem.IsUpdateAccess = item.IsUpdateAccess;
                    existingItem.IsReadAccess = item.IsReadAccess;
                    existingItem.IsDeleteAccess = item.IsDeleteAccess;
                    existingItem.CreatedOn = item.CreatedOn;
                    existingItem.CreatedBy = item.CreatedBy;
                }
            }

            if (sortOrder.ToLower(CultureInfo.InvariantCulture) == "descending")
            {
                return new RolePermissionVM() { ControllerObjects =  controllerArray?.ToList(),  RolePermissions = rolePermissionList.OrderByDescending(m => m.ModuleName).ToList() };
            }
            else
            {
                return new RolePermissionVM { ControllerObjects = controllerArray?.ToList(), RolePermissions = rolePermissionList.OrderBy(m => m.ModuleName).ToList() };
            }
        }
    }
}
