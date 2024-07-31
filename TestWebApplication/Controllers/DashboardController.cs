using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using TestWebApplication.Data;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext context;
        public DashboardController(ApplicationDbContext context)
        {
            this.context = context;
            ViewBag.project = context.Project.ToList();
        }
        public IActionResult Index()
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
                    ViewBag.ListCountAll = context.Incident.Where(t => t.IssueID != 0).Count();
                    ViewBag.ListCountOpen = context.Incident.Where(t => t.IssueID != 0 && t.Status == "Open").Count();
                    ViewBag.ListCountClosed = context.Incident.Where(t => t.IssueID != 0 && t.Status == "Closed").Count();
                    ViewBag.ListCountIP = context.Incident.Where(t => t.IssueID != 0 && t.Status == "In Progress").Count();
                    ViewBag.ListCountPause = context.Incident.Where(t => t.IssueID != 0 && t.Status == "Pause").Count();
                    ViewBag.CountCustomer = context.Customer.Count();
                    ViewBag.CountProject = context.Project.Count();
                    ViewBag.CountModule = context.Module.Count();
                    ViewBag.CountIssueType = context.IssueType.Count();
                    ViewBag.IncidentList = context.Incident.Where(t => t.IssueID != 0).OrderBy(t => t.CreatedOn).ToList();
                    ViewBag.BreachIncident = context.Incident.Where(t => t.HasBreached == true).OrderBy(t => t.CreatedOn).ToList();
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

                    var incident = new List<Incident>();
                    var breachIncident = new List<Incident>();
                    var openIncident = new List<Incident>();
                    var closeIncident = new List<Incident>();
                    var inprogressIncident = new List<Incident>();
                    var pauseIncident = new List<Incident>();
                    foreach (var prjId in projectIDs)
                    {
                        var incidentList = context.Incident.Where(t => t.IssueID != 0 && t.ProjectID == prjId.ProjectID).OrderBy(t => t.CreatedOn).ToList();
                        incident.AddRange(incidentList);
                        var breachIncidentList = context.Incident.Where(t => t.HasBreached == true && t.ProjectID == prjId.ProjectID).OrderBy(t => t.CreatedOn).ToList();
                        breachIncident.AddRange(breachIncidentList);
                        var openIncidentList = context.Incident.Where(t => t.IssueID != 0 && t.ProjectID == prjId.ProjectID && t.Status == "Open");
                        openIncident.AddRange(openIncidentList);
                        var closeIncidentList = context.Incident.Where(t => t.IssueID != 0 && t.ProjectID == prjId.ProjectID && t.Status == "Closed");
                        closeIncident.AddRange(closeIncidentList);
                        var inprogressIncidentList = context.Incident.Where(t => t.IssueID != 0 && t.ProjectID == prjId.ProjectID && t.Status == "InProgress");
                        inprogressIncident.AddRange(inprogressIncidentList);
                        var pauseIncidentList = context.Incident.Where(t => t.IssueID != 0 && t.ProjectID == prjId.ProjectID && t.Status == "Pause");
                        pauseIncident.AddRange(pauseIncidentList);
                    }
                    ViewBag.ListCountAll = incident.Count();
                    ViewBag.ListCountOpen = openIncident.Count();
                    ViewBag.ListCountClosed = closeIncident.Count();
                    ViewBag.ListCountIP = inprogressIncident.Count();
                    ViewBag.ListCountPause = pauseIncident.Count();
                    ViewBag.IncidentList = incident;
                    ViewBag.CountCustomer = context.Customer.Count();
                    ViewBag.CountProject = context.Project.Count();
                    ViewBag.CountModule = context.Module.Count();
                    ViewBag.CountIssueType = context.IssueType.Count();
                    ViewBag.BreachIncident = breachIncident;
                }
                var result = context.Incident.ToList();
                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
