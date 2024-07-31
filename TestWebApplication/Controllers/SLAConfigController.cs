using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TestWebApplication.Common;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    public class SLAConfigController : BaseController
    {
        private readonly ApplicationDbContext context;
        public SLAConfigController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        [RolePermissionAuthorize("Read")]
        public IActionResult Index()
        {
            ViewBag.Status = context.Severity.ToList();
            ViewBag.Priority = context.Severity.Where(p => p.Priority != null).ToList();
            ViewBag.ListCount = context.SLAConfig.Count();

            var SLAlist = context.SLAConfig.OrderBy(m => m.SLAName).ToList();

            return View(SLAlist);
        }
        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public async Task<JsonResult> Index(int Status, int SLAmin, string Priority)
        {
            ViewBag.Status = context.Severity.ToList();
            ViewBag.Priority = context.Severity.Where(p => p.Priority != null).ToList();
            Dictionary<string, string> dt1 = new Dictionary<string, string>();
            var slaType = string.Empty;
            if (Status == 2 || Status == 5)
                slaType = "Resolution";
            else if (Status == 1 || Status == 3 || Status == 4)
                slaType = "Response";
            var slaName = context.Severity.Where(u => u.No == Status).Select(u => u.IncidentStatus).FirstOrDefault();
            SLAConfig sla = new()
            {
                SLAName = slaName,
                StatusID = Status,
                SLAmin = SLAmin,
                Priority = Priority,
                SLAType = slaType,
                CreatedOn = DateTime.Now
            };

            context.SLAConfig.Add(sla);
            context.SaveChanges();

            return Json(dt1);
        }

        [HttpPost, ActionName("Delete")]
        [RolePermissionAuthorize("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            try
            {
                if (context.SLAConfig == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.SLAConfig' is null.");
                }

                //var sla = await context.SLAConfig.Where(s => s.Id == Id).FirstOrDefaultAsync();

                //var proj = await context.Project.FirstOrDefaultAsync(m => m.ProjectID == ProjectID);
                //var incident = await context.Incident.FirstOrDefaultAsync(m => m.ProjectID == proj.ProjectID);
                //var module = await context.Module.FirstOrDefaultAsync(m => m.ProjectID == proj.ProjectID);
                //if (incident != null || module != null)
                //{
                //    TempData["ErrorMsg"] = "Already used. Cannot Delete.";
                //    return RedirectToAction(nameof(Index));
                //}
                //var project = await context.Project.FindAsync(ProjectID);
                var sla = await context.SLAConfig.FindAsync(Id);
                if (sla != null)
                {
                    context.SLAConfig.Remove(sla);
                }
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public JsonResult Save(int SLAMin, string SLAName)
        {
            Dictionary<string, string> dt1 = new Dictionary<string, string>();

            var sla = context.SLAConfig.Where(t => t.SLAName == SLAName).FirstOrDefault();

            if (sla != null)
            {
                sla.SLAmin = SLAMin;
                context.SLAConfig.Update(sla);
                context.SaveChanges();
                dt1.Add(UploadConstants.MessageKey, "Success");
            }
            else
                dt1.Add(UploadConstants.MessageKey, "Fail");

            return Json(dt1);
        }

    }
}
