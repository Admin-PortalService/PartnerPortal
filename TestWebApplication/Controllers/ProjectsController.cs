using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TestWebApplication.Common;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;
namespace TestWebApplication.Controllers
{
    [Authorize]
    public class ProjectsController : BaseController
    {
        private readonly ApplicationDbContext context;
        public ProjectsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [RolePermissionAuthorize("Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.ListCount = await context.Project.CountAsync();
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
                ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0 && t.IsActive == true).OrderBy(t => t.CustomerName).ToListAsync();
                ViewBag.Status = await context.Severity.Where(t => t.PrjStatus != null).ToListAsync();
                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> Create(Projects model)
        {
            try
            {
                string userName = User.Identity?.Name;
                var LoginName = await context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefaultAsync();
                if (ModelState.IsValid)
                {
                    var prjCode = await context.Project.Where(p => p.ProjectCode == model.ProjectCode).ToListAsync();
                    var prjName = await context.Project.Where(p => p.ProjectName == model.ProjectName).ToListAsync();
                    if (prjCode.Count == 0 && prjName.Count == 0)
                    {
                        Projects obj = new()
                        {
                            ProjectCode = model.ProjectCode,
                            ProjectName = model.ProjectName,
                            ProjectDesc = model.ProjectDesc,
                            CustomerID = model.CustomerID,
                            ValidFrom = model.ValidFrom,
                            ValidTo = model.ValidTo,
                            LastDate = model.LastDate,
                            Status = model.Status,
                            IsActive = true,
                            CreatedBy = LoginName.UserName,
                            ModifiedBy = LoginName.UserName,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                        };

                        context.Project.Add(obj);
                        context.SaveChanges();

                        //var project = context.Project.SingleOrDefault(p => p.ProjectName == obj.ProjectName);
                        var project = await context.Project.Where(p => p.ProjectName == obj.ProjectName).FirstOrDefaultAsync();
                        MaintenanceLog maintainLog = new()
                        {
                            ProjectID = project.ProjectID,
                            ProjectName = project.ProjectName,
                            CreatedBy = LoginName.UserName,
                            CreatedOn = DateTime.Now,
                            ModifiedBy = LoginName.UserName,
                            ModifiedOn = DateTime.Now,
                            LastMaintainDate = model.LastDate,
                        };
                        context.MaintenanceLog.Add(maintainLog);

                        context.SaveChanges();

                        TempData["Success"] = "Project Successfully Created";

                        return RedirectToAction("Index");
                    }
                    else if (prjCode.Count == 0)
                    {
                        ViewBag.error = "Project Name already Exist";
                        ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0 && t.IsActive == true).OrderBy(t => t.CustomerName).ToListAsync();
                        ViewBag.Status = await context.Severity.Where(t => t.PrjStatus != null).ToListAsync();
                        return View();
                    }
                    else
                    {
                        ViewBag.error = "Project Code already Exist";
                        ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0 && t.IsActive == true).OrderBy(t => t.CustomerName).ToListAsync();
                        ViewBag.Status = await context.Severity.Where(t => t.PrjStatus != null).ToListAsync();
                        return View();
                    }

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
                    ModelState.Clear();
                    ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0 && t.IsActive == true).OrderBy(t => t.CustomerName).ToListAsync();
                    ViewBag.Status = await context.Severity.Where(t => t.PrjStatus != null).ToListAsync();
                    return View();

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
                var objPrj = context.Project.SingleOrDefault(m => m.ProjectID == Id);
                var result = new Projects()
                {
                    ProjectID = objPrj.ProjectID,
                    ProjectCode = objPrj.ProjectCode,
                    CustomerID = objPrj.CustomerID,
                    ProjectName = objPrj.ProjectName,
                    ProjectDesc = objPrj.ProjectDesc,
                    ValidFrom = objPrj.ValidFrom,
                    ValidTo = objPrj.ValidTo,
                    LastDate = objPrj.LastDate,
                    Status = objPrj.Status,
                    CreatedBy = objPrj.CreatedBy,
                    CreatedOn = objPrj.CreatedOn,
                    ModifiedBy = objPrj.ModifiedBy,
                    ModifiedOn = DateTime.Now
                };
                //ViewBag.CustomerList = context.Customer.SingleOrDefault(t => t.CustomerID == result.CustomerID);
                ViewBag.CustomerList = context.Customer.ToList();
                ViewBag.Customer = context.Customer.SingleOrDefault(t => t.CustomerID == result.CustomerID);
                ViewBag.statusList = context.Severity.Where(t => t.PrjStatus != null).ToList();
                ViewBag.ProjStatus = objPrj.Status;
                return View(result);

            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(Projects prj)
        {
            try
            {
                string userName = User.Identity?.Name;
                var LoginName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                var objPrj = new Projects()
                {
                    ProjectID = prj.ProjectID,
                    ProjectCode = prj.ProjectCode,
                    CustomerID = prj.CustomerID,
                    ProjectName = prj.ProjectName,
                    ProjectDesc = prj.ProjectDesc,
                    ValidFrom = prj.ValidFrom,
                    ValidTo = prj.ValidTo,
                    LastDate = prj.LastDate,
                    Status = prj.Status,
                    IsActive = true,
                    CreatedBy = prj.CreatedBy,
                    CreatedOn = prj.CreatedOn,
                    ModifiedBy = LoginName.UserName,
                    ModifiedOn = DateTime.Now
                };

                var project = context.Project.Where(p => p.ProjectID == prj.ProjectID).AsNoTracking().FirstOrDefault();
                if (project.LastDate != prj.LastDate)
                {
                    var objMLog = new MaintenanceLog()
                    {
                        ProjectID = prj.ProjectID,
                        ProjectName = prj.ProjectName,
                        CreatedBy = prj.CreatedBy,
                        CreatedOn = DateTime.Now,
                        ModifiedBy = LoginName.UserName,
                        ModifiedOn = DateTime.Now,
                        LastMaintainDate = prj.LastDate
                    };
                    context.MaintenanceLog.Update(objMLog);
                }

                context.Project.Update(objPrj);
                context.SaveChanges();

                TempData["Success"] = "Project Successfully Modified";

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
                IEnumerable<Projects> query;
                if (sortOrder.ToLower(CultureInfo.InvariantCulture) == "descending")
                {
                    query = context.Project.OrderByDescending(m => m.ProjectName).AsEnumerable();
                }
                else
                {
                    query = context.Project.OrderBy(m => m.ProjectName);
                }
                if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchString))
                {
                    if (searchType.Equals(SearchTypeConstant.CONTAIN, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.ProjectName is not null && m.ProjectName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.ProjectDesc is not null && m.ProjectDesc.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.STARTWITH, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.ProjectName is not null && m.ProjectName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                            (m.ProjectDesc is not null && m.ProjectDesc.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.ENDWITH, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.ProjectName is not null && m.ProjectName.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.ProjectDesc is not null && m.ProjectDesc.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.EQUAL, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.ProjectName is not null && m.ProjectName.Equals(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.ProjectDesc is not null && m.ProjectDesc.Equals(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                }
                // Extract a portion of data
                List<Projects> model = query.Skip(firstItem).Take(BATCHSIZE).ToList();
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
                if (id == null || context.Project == null)
                { return NotFound(); }

                var project = await context.Project.FirstOrDefaultAsync(m => m.ProjectID == id);
                if (project == null)
                { return NotFound(); }

                return View(project);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost, ActionName("Delete")]
        [RolePermissionAuthorize("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ProjectID)
        {
            try
            {
                if (context.Project == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Project' is null.");
                }
                var proj = await context.Project.FirstOrDefaultAsync(m => m.ProjectID == ProjectID);
                var incident = await context.Incident.FirstOrDefaultAsync(m => m.ProjectID == proj.ProjectID);
                var module = await context.Module.FirstOrDefaultAsync(m => m.ProjectID == proj.ProjectID);
                if (incident != null || module != null)
                {
                    TempData["ErrorMsg"] = "Already used. Cannot Delete.";
                    return RedirectToAction(nameof(Index));
                    //return Problem("Already used in Incident. Cannot Delete!!");
                }
                var project = await context.Project.FindAsync(ProjectID);
                if (project != null)
                {
                    context.Project.Remove(project);
                }
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                if (id == null || context.Project == null)
                {
                    return NotFound();
                }
                var project = await context.Project.FirstOrDefaultAsync(m => m.ProjectID == id);
                ViewBag.Organization = await context.Customer.FirstOrDefaultAsync(t => t.CustomerID == project.CustomerID);
                if (project == null)
                {
                    return NotFound();
                }
                return View(project);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public async Task<IActionResult> LogHistory()
        {
            try
            {
                ViewBag.maintenanceLog = await context.MaintenanceLog.ToListAsync();
                ViewBag.ProjectList = await context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToListAsync();
                //var project = await context.Project.Where()
                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public JsonResult FilteringProject(int projectId)
        {
            try
            {
                var maintainLog = context.MaintenanceLog.Where(m => m.ProjectID == projectId).ToList();

                return Json(maintainLog);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
