using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using NuGet.Protocol.Core.Types;
using System.Globalization;
using TestWebApplication.Data;
using TestWebApplication.Models.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq;
using TestWebApplication.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TestWebApplication.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class RolesController  : BaseController
    {
        private readonly ApplicationDbContext context;

        public RolesController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [RolePermissionAuthorize("Read")]
        public async Task<IActionResult> Index()
        {
            ViewBag.ListCount = await context.Roles.Where(t => t.RoleType != "SuperAdmin").CountAsync();
            if (TempData["Success"] != null)
            {
                ViewBag.msg = TempData["Success"];
            }
            return View();
        }
        [HttpGet]
        [RolePermissionAuthorize("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public IActionResult Create(Roles model)
        {
            string userName = User.Identity?.Name;
            var LoginName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

            if (ModelState.IsValid)
            {
                Roles obj = new()
                {
                    RoleType = model.RoleType,
                    RoleDesc = model.RoleDesc,
                    IsInternal = model.IsInternal,
                    IsActive = model.IsActive,
                    CreatedBy = LoginName.UserName,
                    ModifiedBy = LoginName.UserName,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };

                context.Roles.Add(obj);
                context.SaveChanges();

                TempData["Success"] = "Role Successfully Created";

                return RedirectToAction("Index");

            }
            else
            {
                var errors = new Dictionary<string, IEnumerable<string>>();
                List<string> message = new List<string>();
                foreach (var keyValue in ModelState)
                {
                    if (keyValue.Value.Errors.Count > 0)
                    {
                        errors[keyValue.Key] = keyValue.Value.Errors.Select(e => e.ErrorMessage);//ValidationState
                        var msg1 = keyValue.Key;
                        var msg2 = keyValue.Value.ValidationState;
                        string msg = msg1 + new string(' ', 3) + "is" + new string(' ', 3) + msg2;
                        message.Add(msg);
                    }
                }
                var strings = (from o in message
                               select o.ToString()).ToList();
                ViewBag.msgList = strings;
                //TempData["msg"] = message;
                ModelState.Clear();
                return View(model);

            }

        }

        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(Guid Id)
        {
            var objRole = context.Roles.SingleOrDefault(m => m.RoleID == Id);
            var result = new Roles()
            {
                RoleID = objRole.RoleID,
                RoleType = objRole.RoleType,
                RoleDesc = objRole.RoleDesc,
                IsInternal = objRole.IsInternal,
                IsActive = objRole.IsActive,
                CreatedBy = objRole.CreatedBy,
                CreatedOn = objRole.CreatedOn,
                ModifiedBy = objRole.ModifiedBy,
                ModifiedOn = DateTime.Now
            };

            return View(result);
        }
        [HttpPost]
        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(Roles role)
        {
            string userName = User.Identity?.Name;
            var LoginName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();
            var objRole = new Roles()
            {
                RoleID = role.RoleID,
                RoleType = role.RoleType,
                RoleDesc = role.RoleDesc,
                IsInternal = role.IsInternal,
                IsActive = role.IsActive,
                CreatedBy = role.CreatedBy,
                CreatedOn = role.CreatedOn,
                ModifiedBy = LoginName.UserName,
                ModifiedOn = DateTime.Now
            };
            context.Roles.Update(objRole);
            context.SaveChanges();
            TempData["Success"] = "Role is successfully modified!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> _ListDataPartial(string sortOrder, string searchType, string searchString, int firstItem = 0)
        {
            //Sort and filter test data
            IEnumerable<Roles> query;
            if (sortOrder.ToLower(CultureInfo.InvariantCulture) == "descending")
            {
                query = context.Roles.Where(t => t.RoleType != "SuperAdmin").OrderByDescending(m => m.RoleType).AsEnumerable();
            }
            else
            {
                query = context.Roles.Where(t => t.RoleType != "SuperAdmin").OrderBy(m => m.RoleType);
            }
            if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchString))
            {
                if (searchType.Equals(SearchTypeConstant.CONTAIN, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    (m.RoleType is not null && m.RoleType.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (m.RoleDesc is not null && m.RoleDesc.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                }
                else if (searchType.Equals(SearchTypeConstant.STARTWITH, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    (m.RoleType is not null && m.RoleType.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.RoleDesc is not null && m.RoleDesc.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                }
                else if (searchType.Equals(SearchTypeConstant.ENDWITH, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    (m.RoleType is not null && m.RoleType.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (m.RoleDesc is not null && m.RoleDesc.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                }
                else if (searchType.Equals(SearchTypeConstant.EQUAL, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    (m.RoleType is not null && m.RoleType.Equals(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (m.RoleDesc is not null && m.RoleDesc.Equals(searchString, StringComparison.OrdinalIgnoreCase)));
                }
            }
            // Extract a portion of data
            List<Roles> model = query.Skip(firstItem).Take(BATCHSIZE).ToList();
            if (model.Count == 0)
            {
                return StatusCode(204); // 204 := "No Content"
            }

            return PartialView(model);

        }
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null || context.Roles == null)
            { return NotFound(); }

            var role = await context.Roles.FirstOrDefaultAsync(m => m.RoleID == id);
            if (role == null)
            { return NotFound(); }

            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [RolePermissionAuthorize("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid RoleID)
        {
            if (context.Roles == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Roles' is null.");
            }
            var role = await context.Roles.FindAsync(RoleID);
            if (role != null)
            {
                context.Roles.Remove(role);
            }
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
