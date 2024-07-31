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
using TestWebApplication.Helpers;
using Microsoft.Build.Evaluation;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class ModuleController : BaseController
    {
        private readonly ApplicationDbContext context;
        public ModuleController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [RolePermissionAuthorize("Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userName = User.Identity?.Name;
                //var Role = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();

                var countList = new List<Module>();
                int count = 0;
                if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    ViewBag.ListCount = await context.Module.CountAsync();
                }
                else
                {
                    var userID = context.UserAccess.Where(t => t.UserMail == userName).Select(t => t.UserID).FirstOrDefault();
                    var customerIDs = context.User_Customer.Where(u => u.UserID == userID).ToList();

                    var projectIDs = new List<Projects>();
                    foreach (var cusId in customerIDs)
                    {
                        projectIDs = context.Project.Where(t => t.CustomerID == cusId.CustomerID).ToList();
                    }

                    foreach (var projectID in projectIDs)
                    {
                        var moduleList = await context.Module.Where(i => i.ModuleID != 0 && i.ProjectID == projectID.ProjectID).ToListAsync();
                        countList.AddRange(moduleList);
                        count = countList.Count;
                    }
                    ViewBag.ListCount = count;
                }

                if (TempData["Success"] != null)
                {
                    ViewBag.msg = TempData["Success"];
                }

                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userName = User.Identity?.Name;
                //var Role = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();

                if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    ViewBag.ProjectList = await context.Project.OrderBy(t => t.ProjectDesc).ToListAsync();
                }
                else
                {

                    var userID = context.UserAccess.Where(t => t.UserMail == userName).Select(t => t.UserID).FirstOrDefault();
                    var customerIDs = context.User_Customer.Where(u => u.UserID == userID).ToList();

                    foreach (var cusId in customerIDs)
                    {
                        ViewBag.ProjectList = context.Project.Where(t => t.CustomerID == cusId.CustomerID).ToList();
                    }
                                      
                }
                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public IActionResult Create(Module model)
        {
            try
            {

                string userName = User.Identity?.Name;
                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();
                var LoginName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    Module obj = new()
                    {
                        ProjectID = model.ProjectID,
                        ModuleName = model.ModuleName,
                        ModuleDesc = model.ModuleDesc,
                        CreatedBy = LoginName.UserName,
                        ModifiedBy = LoginName.UserName,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    };

                    context.Module.Add(obj);
                    context.SaveChanges();

                    TempData["Success"] = "Module Successfully Created.";

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

                    if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                    {
                        ViewBag.ProjectList = context.Project.OrderBy(t => t.ProjectDesc).ToList();
                    }
                    else
                    {
                        var userID = context.UserAccess.Where(t => t.UserMail == userName).Select(t => t.UserID).FirstOrDefault();
                        var customerIDs = context.User_Customer.Where(u => u.UserID == userID).ToList();

                        var projectIDs = new List<Projects>();
                        foreach (var cusId in customerIDs)
                        {
                            ViewBag.ProjectList = context.Project.Where(t => t.CustomerID == cusId.CustomerID).OrderBy(t => t.ProjectDesc).ToList();
                        }
                    }

                    //TempData["msg"] = message;
                    ModelState.Clear();
                    return View(model);

                }

            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(int Id)
        {
            try
            {
                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userName = User.Identity?.Name;
                //var Role = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();

                if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    ViewBag.ProjectList = context.Project.OrderBy(t => t.ProjectDesc).ToList();
                }
                else
                {
                    var userID = context.UserAccess.Where(t => t.UserMail == userName).Select(t => t.UserID).FirstOrDefault();
                    var customerIDs = context.User_Customer.Where(u => u.UserID == userID).ToList();

                    var projectIDs = new List<Projects>();
                    foreach (var cusId in customerIDs)
                    {
                        ViewBag.ProjectList = context.Project.Where(t => t.CustomerID == cusId.CustomerID).OrderBy(t => t.ProjectDesc).ToList();
                    }

                }
                var objModule = context.Module.SingleOrDefault(m => m.ModuleID == Id);
                var result = new Module()
                {
                    ProjectID = objModule.ProjectID,
                    ModuleID = objModule.ModuleID,
                    ModuleName = objModule.ModuleName,
                    ModuleDesc = objModule.ModuleDesc,
                    CreatedBy = objModule.CreatedBy,
                    CreatedOn = objModule.CreatedOn,
                    ModifiedBy = objModule.ModifiedBy,
                    ModifiedOn = DateTime.Now
                };

                ViewBag.selectedProject = context.Project.Where(p => p.ProjectID == result.ProjectID).FirstOrDefault();

                return View(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpPost]
        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(Module mod)
        {
            try
            {
                string userName = User.Identity?.Name;
                var LoginName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();
                var objModule = new Module()
                {
                    ModuleID = mod.ModuleID,
                    ModuleName = mod.ModuleName,
                    ModuleDesc = mod.ModuleDesc,
                    CreatedBy = mod.CreatedBy,
                    CreatedOn = mod.CreatedOn,
                    ModifiedBy = LoginName.UserName,
                    ModifiedOn = DateTime.Now
                };
                context.Module.Update(objModule);
                context.SaveChanges();
                //alert msg
                TempData["Success"] = "Module Successfully Modified.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
     
        [HttpPost]
        public async Task<IActionResult> _ListDataPartial(string sortOrder, string searchType, string searchString, int firstItem = 0)
        {
            try
            {

                //Sort and filter test data
                IEnumerable<Module> query = new List<Module>();
                IEnumerable<Module> module = new List<Module>();
                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userName = User.Identity?.Name;
                //var Role = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();

                if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    if (sortOrder.ToLower(CultureInfo.InvariantCulture) == "descending")
                    {
                        module = context.Module.OrderByDescending(m => m.ModuleName).AsEnumerable();
                    }
                    else
                    {
                        module = context.Module.OrderBy(m => m.ModuleName);
                    }
                }
                else
                {
                    var userID = context.UserAccess.Where(t => t.UserMail == userName).Select(t => t.UserID).FirstOrDefault();
                    var customerIDs = context.User_Customer.Where(u => u.UserID == userID).ToList();

                    var projectIDs = new List<Projects>();
                    foreach (var cusId in customerIDs)
                    {
                        projectIDs = context.Project.Where(t => t.CustomerID == cusId.CustomerID).ToList();
                    }

                    foreach (var projectID in projectIDs)
                    {
                        if (sortOrder.ToLower(CultureInfo.InvariantCulture) == "descending")
                        {
                            query = context.Module
                                .Where(i => i.ModuleID != 0 && i.ProjectID == projectID.ProjectID)
                                .OrderByDescending(m => m.ModuleName).AsEnumerable();
                            module = module.Concat(query).ToList();
                        }
                        else
                        {
                            query = context.Module
                                .Where(i => i.ModuleID != 0 && i.ProjectID == projectID.ProjectID)
                                .OrderBy(m => m.ModuleName);
                            module = module.Concat(query).ToList();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchString))
                {
                    if (searchType.Equals(SearchTypeConstant.CONTAIN, StringComparison.OrdinalIgnoreCase))
                    {
                        module = module.Where(m =>
                        (m.ModuleName is not null && m.ModuleName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.ModuleDesc is not null && m.ModuleDesc.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.STARTWITH, StringComparison.OrdinalIgnoreCase))
                    {
                        module = module.Where(m =>
                        (m.ModuleName is not null && m.ModuleName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                            (m.ModuleDesc is not null && m.ModuleDesc.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.ENDWITH, StringComparison.OrdinalIgnoreCase))
                    {
                        module = module.Where(m =>
                        (m.ModuleName is not null && m.ModuleName.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.ModuleDesc is not null && m.ModuleDesc.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.EQUAL, StringComparison.OrdinalIgnoreCase))
                    {
                        module = module.Where(m =>
                        (m.ModuleName is not null && m.ModuleName.Equals(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.ModuleDesc is not null && m.ModuleDesc.Equals(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                }
                // Extract a portion of data
                List<Module> model = module.Skip(firstItem).Take(BATCHSIZE).ToList();
                if (model.Count == 0)
                {
                    return StatusCode(204); // 204 := "No Content"
                }

                return PartialView(model);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [RolePermissionAuthorize("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == null || context.Module == null)
                { return NotFound(); }

                var module = await context.Module.FirstOrDefaultAsync(m => m.ModuleID == id);
                if (module == null)
                { return NotFound(); }

                return View(module);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost, ActionName("Delete")]
        [RolePermissionAuthorize("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ModuleID)
        {
            try
            {
                if (context.Module == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Module' is null.");
                }
                var modulee = await context.Module.FirstOrDefaultAsync(m => m.ModuleID == ModuleID);
                var incident = await context.Incident.FirstOrDefaultAsync(m => m.ModuleID == modulee.ModuleID);
                if (incident != null)
                {
                    TempData["ErrorMsg"] = "Already used in Incident. Cannot Delete.";
                    return RedirectToAction(nameof(Index));
                }
                var module = await context.Module.FindAsync(ModuleID);
                if (module != null)
                {
                    context.Module.Remove(module);
                }
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

