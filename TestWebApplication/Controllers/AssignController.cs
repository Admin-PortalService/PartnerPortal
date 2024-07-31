using Microsoft.AspNetCore.Mvc;
using TestWebApplication.Data;
using TestWebApplication.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using TestWebApplication.Helpers;
using System.Net.Mail;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class AssignController : Controller
    {
        private readonly ApplicationDbContext context;
        public AssignController(ApplicationDbContext context)
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
                if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    ViewBag.IncUnAssign = context.Incident.Where(i => i.IssueID != 0 && i.IsAssigned == false).OrderBy(i => i.CreatedOn).ToList();
                    ViewBag.IncAssign = context.Incident.Where(i => i.IssueID != 0 && i.IsAssigned == true).OrderBy(i => i.CreatedOn).ToList();
                    ViewBag.Incident = context.Incident.Where(i => i.IssueID != 0).OrderBy(i => i.IssueID).ToList();
                    ViewBag.ModuleList = context.Module.Where(t => t.ModuleID != 0).OrderBy(t => t.ModuleName).ToList();
                    ViewBag.ProjectList = context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToList();
                    ViewBag.PriorityList = context.Severity.Where(r => r.Priority != "" && r.Priority != null).OrderBy(r => r.No).ToList();
                    ViewBag.IssueTypeList = context.IssueType.Where(i => i.IssueTypeID != 0).OrderBy(i => i.IssueTypeName).ToList();
                    ViewBag.StatusList = context.Severity.Where(s => s.No != 0).OrderBy(s => s.IncidentStatus).ToList();
                    ViewBag.CreatedByList = context.UserAccess.Where(c => c.UserID != 0).OrderBy(c => c.CreatedBy).ToList();
                    ViewBag.AssigneeList = context.Assign.Where(a => a.AssignID != 0).OrderBy(a => a.UserName).ToList();
                }
                else
                {
                    var userID = context.UserAccess.Where(t => t.UserMail == userName).Select(t => t.UserID).FirstOrDefault();
                    //var projectID = context.Project.Where(t => t.CustomerID == customerID.CustomerID).FirstOrDefault();
                    //ViewBag.IncUnAssign = context.Incident.Where(i => i.IssueID != 0 && i.ProjectID == projectID.ProjectID && i.IsAssigned == false).OrderBy(i => i.CreatedOn).ToList();
                    //ViewBag.IncAssign = context.Incident.Where(i => i.IssueID != 0 && i.ProjectID == projectID.ProjectID && i.IsAssigned == true).OrderBy(i => i.CreatedOn).ToList();
                    //ViewBag.Incident = context.Incident.Where(i => i.IssueID != 0 && i.ProjectID == projectID.ProjectID).OrderBy(i => i.IssueID).ToList();

                    var customerIDs = context.User_Customer.Where(u => u.UserID == userID).ToList();

                    var projectIDs = new List<Projects>();
                    foreach (var cusId in customerIDs)
                    {
                        projectIDs = context.Project.Where(t => t.CustomerID == cusId.CustomerID).ToList();
                    }
                    var incidents = new List<Incident>();
                    var unAssignInc = new List<Incident>();
                    var assignInc = new List<Incident>();

                    foreach (var projectID in projectIDs)
                    {
                        var incidentUnAssign = context.Incident
                            .Where(i => i.IssueID != 0 && i.ProjectID == projectID.ProjectID && i.IsAssigned == false)
                            .OrderBy(i => i.CreatedOn)
                            .ToList();

                        var incidentAssign = context.Incident
                            .Where(i => i.IssueID != 0 && i.ProjectID == projectID.ProjectID && i.IsAssigned == true)
                            .OrderBy(i => i.CreatedOn)
                            .ToList();

                        var incident = context.Incident
                            .Where(i => i.IssueID != 0 && i.ProjectID == projectID.ProjectID)
                            .OrderBy(i => i.IssueID).ToList();

                        unAssignInc.AddRange(incidentUnAssign);
                        assignInc.AddRange(incidentAssign);
                        incidents.AddRange(incident);
                    }

                    ViewBag.IncUnAssign = unAssignInc;
                    ViewBag.IncAssign = assignInc;
                    ViewBag.Incident = incidents;
                    ViewBag.ModuleList = context.Module.Where(t => t.ModuleID != 0).OrderBy(t => t.ModuleName).ToList();
                    ViewBag.ProjectList = context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToList();
                    ViewBag.PriorityList = context.Severity.Where(r => r.Priority != "" && r.Priority != null).OrderBy(r => r.No).ToList();
                    ViewBag.IssueTypeList = context.IssueType.Where(i => i.IssueTypeID != 0).OrderBy(i => i.IssueTypeName).ToList();
                    ViewBag.StatusList = context.Severity.Where(s => s.No != 0).OrderBy(s => s.IncidentStatus).ToList();
                    ViewBag.CreatedByList = context.UserAccess.Where(c => c.UserID != 0).OrderBy(c => c.CreatedBy).ToList();
                    ViewBag.AssigneeList = context.Assign.Where(a => a.AssignID != 0).OrderBy(a => a.UserName).ToList();

                }
                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public async Task<IActionResult> AssignMe() /*string name*/
        {
            try
            {
                ViewBag.UserMail = Base64Encode(User.Identity?.Name);
                string UserDecode = Base64Decode(ViewBag.UserMail);
                var Assign = context.Assign.Where(a => a.UserMail == UserDecode).ToList();
                List<Incident> incList = new List<Incident>();
                foreach (var obj in Assign)
                {
                    var List = context.Incident.SingleOrDefault(i => i.IssueID == obj.IssueID); //CustomerID UserAccess
                    incList.Add(List);
                }
                var Lists = (from o in incList
                             select o).ToList();
                ViewBag.IncList = Lists;
                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> Create(int Id)
        {
            try
            {
                ViewBag.IncidentDetails = await context.Incident.Where(t => t.IssueID == Id).ToListAsync();
                if (Id == null || context.Incident == null)
                { return NotFound(); }
                var inc = await context.Incident.FirstOrDefaultAsync(m => m.IssueID == Id);
                ViewBag.module = context.Module.SingleOrDefault(t => t.ModuleID == inc.ModuleID);
                ViewBag.project = context.Project.SingleOrDefault(t => t.ProjectID == inc.ProjectID);
                var project = context.Project.SingleOrDefault(t => t.ProjectID == inc.ProjectID);

                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userName = User.Identity?.Name;
                //var Role = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();
                if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    ViewBag.Incident = context.Incident.Where(x => x.IssueID == Id).ToList();
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

                    var ind = new List<Incident>();
                    foreach (var projectID in projectIDs)
                    {
                        var incident = context.Incident
                             .Where(i => i.IssueID == Id && i.ProjectID == projectID.ProjectID);

                        ind.AddRange(incident);
                    }
                    ViewBag.Incident = ind;
                }

                var usrIds = context.User_Customer.Where(u => u.CustomerID == project.CustomerID).Select(u => u.UserID).ToList();
                var userList = new List<UserAccess>();
                foreach (var usrId in usrIds)
                {
                    var usrList = await context.UserAccess.Where(t => t.UserID == usrId && (t.RoleName.Contains("Consult") || t.RoleName.Contains("Tech"))).ToListAsync();
                    userList.AddRange(usrList);
                }
                ViewBag.assignList = userList;
                ViewBag.asignee = context.Assign.SingleOrDefault(t => t.IssueID == Id);

                if (inc == null)
                { return NotFound(); }
                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Create(Assign model)
        {

            try
            {
                var user = HttpContext.User;
                //adding for assign button (moving get to post)
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var UserInfo = context.UserAccess.SingleOrDefault(t => t.UserID == model.UserID);
                var inc = context.Incident.FirstOrDefault(m => m.IssueID == model.IssueID);
                var project = context.Project.SingleOrDefault(t => t.ProjectID == inc.ProjectID);


                var issue = context.Assign.FirstOrDefault(x => x.IssueID == model.IssueID);
                //adding for assign button
                ViewBag.project = context.Project.SingleOrDefault(t => t.ProjectID == inc.ProjectID);
                ViewBag.module = context.Module.SingleOrDefault(t => t.ModuleID == inc.ModuleID);
                ViewBag.IncidentDetails = context.Incident.Where(t => t.IssueID == model.IssueID).ToList();

                var assignLog = context.AssignLog.Where(a => a.IssueID == model.IssueID).OrderByDescending(i => i.AssignLogID).FirstOrDefault();

                //adding for assign button(moving get to post)
                List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();
                if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    ViewBag.Incident = context.Incident.Where(x => x.IssueID == model.IssueID).ToList();
                }//adding for assign button

                if (ModelState.IsValid)
                {
                    Assign obj = new()
                    {
                        IssueID = model.IssueID,
                        UserID = model.UserID,
                        UserName = UserInfo.UserName,
                        UserMail = UserInfo.UserMail,
                        AssignBy = model.AssignBy,
                        AssignOn = DateTime.Now,
                        Description = model.Description,
                    };
                    if (issue != null)
                    {
                        issue.UserID = obj.UserID;
                        issue.UserName = obj.UserName;
                        issue.UserMail = obj.UserMail;
                        issue.AssignBy = model.AssignBy;
                        issue.AssignOn = DateTime.Now;
                        issue.Description = obj.Description;
                    }
                    else
                        context.Assign.Add(obj);

                    var IsAssign = context.Incident.SingleOrDefault(t => t.IssueID == obj.IssueID);
                    if (IsAssign != null)
                    {
                        IsAssign.IsAssigned = true;
                        IsAssign.Assignee = UserInfo.UserName;
                        IsAssign.Description = model.Description;
                        //context.SaveChanges();
                    }

                    if (assignLog == null || assignLog.AssignTo != IsAssign.Assignee || assignLog.Description != model.Description)
                    {
                        var remark = string.Empty;
                        if (assignLog == null)
                        {
                            remark = "Assign User and Description changed.";
                        }
                        else if (assignLog.AssignTo != IsAssign.Assignee && assignLog.Description != model.Description)
                        {
                            remark = "Assign User and Description changed.";
                        }
                        else if (assignLog.AssignTo != IsAssign.Assignee)
                        {
                            remark = "Assign User changed.";
                        }
                        else
                        {
                            remark = "Description changed.";
                        }

                        AssignLog assign = new()
                        {
                            IssueID = model.IssueID,
                            AssignID = model.AssignID,
                            IssueName = IsAssign.IssueTitle,
                            AssignBy = model.AssignBy,
                            AssignTo = obj.UserName,
                            ModifiedBy = IsAssign.ModifiedBy,
                            ModifiedOn = IsAssign.ModifiedOn,
                            AssignDate = DateTime.Now,
                            Description = model.Description,
                            Remark = remark
                        };
                        context.AssignLog.Add(assign);
                    }

                    context.SaveChanges();
                    var to = new List<string>();
                    var cc = new List<string>();
                    string mail = IsAssign.IssueID + "\n" + IsAssign.IssueTitle;
                    to.Add(obj.UserMail);
                    SendNotificationtoUser("Portal Service", "New Incident Assign", to, cc, mail);

                    //return RedirectToAction("AssignTo", new { @id = obj.IssueID });

                    var usrIds = context.User_Customer.Where(u => u.CustomerID == project.CustomerID).Select(u => u.UserID).ToList();

                    foreach (var usrId in usrIds)
                    {
                        ViewBag.assignList = context.UserAccess.Where(t => t.UserID == usrId && (t.RoleName.Contains("Consult") || t.RoleName.Contains("Tech"))).ToList();
                    }

                    //ViewBag.assignList = context.UserAccess.Where(t => t.CustomerID == project.CustomerID && (t.RoleName.Contains("Consult") || t.RoleName.Contains("Tech"))).ToList();
                    ViewBag.asignee = context.Assign.SingleOrDefault(t => t.IssueID == model.IssueID);
                    return View(model);
                }
                //return RedirectToAction("AssignTo", new { @id = model.IssueID});
                return View(model);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //public async Task<IActionResult> AssignTo(int Id)
        //{       
        //    ViewBag.IncidentDetails = await context.Incident.Where(t => t.IssueID == Id).ToListAsync();
        //    var inc = await context.Incident.FirstOrDefaultAsync(m => m.IssueID == Id);
        //    var assign = await context.Assign.FirstOrDefaultAsync(x => x.IssueID == Id);
        //    ViewBag.module = context.Module.SingleOrDefault(t => t.ModuleID == inc.ModuleID);
        //    ViewBag.project = context.Project.SingleOrDefault(t => t.ProjectID == inc.ProjectID);

        //    ViewBag.asignee = context.Assign.SingleOrDefault(t => t.IssueID == Id);          
        //    return View();
        //}
        public bool SendNotificationtoUser(string name, string subject, List<string> toList, List<string> ccList, string body)
        {
            try
            {
                string sender = "portalservice000@gmail.com";

                MailMessage mail = new MailMessage();

                if (toList != null && toList.Count > 0)
                {
                    mail.IsBodyHtml = true;

                    if (toList != null && toList.Count > 0)
                    {
                        foreach (string email in toList)
                        {
                            mail.To.Add(email);
                        }
                    }

                    if (ccList != null && ccList.Count > 0)
                    {
                        foreach (string email in ccList)
                        {
                            mail.CC.Add(email);
                        }
                    }
                    if (sender != null)
                    {
                        mail.From = new MailAddress(sender);
                    }
                    mail.Subject = subject;
                    mail.Body = body;

                    SendEmailer sendEmailer = new SendEmailer();
                    sendEmailer.SendMail(mail);
                }
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static string Base64Encode(string plainText)
        {
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                string IDDecode = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                return IDDecode;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IActionResult> AssignLog()
        {
            try
            {
                ViewBag.assignLog = await context.AssignLog.ToListAsync();
                ViewBag.IncidentList = await context.Incident.Where(i => i.IssueID != 0).OrderBy(i => i.IssueID).ToArrayAsync();

                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public JsonResult FilteringIssueName(int issueId)
        {
            var assignLog = context.AssignLog.Where(m => m.IssueID == issueId).ToList();

            return Json(assignLog);
        }

        [HttpPost]
        public JsonResult GetAssignList(int ModuleId, int PrjId, /*string IssueType,*/ string Priority, string Status, string Createdby, DateTime? fromDate, DateTime? toDate, int issueid, string assign, bool? assignType)
        {
            try
            {

                // Validate parameters
                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                {
                    // Return appropriate response for invalid date range
                    return Json(new { error = "Invalid date range" });
                }

                // Build the query dynamically based on the provided parameters
                var query = context.Incident.AsQueryable();
                query = query.Where(i =>
                    (PrjId == 0 || i.ProjectID == PrjId) &&
                    (ModuleId == 0 || i.ModuleID == ModuleId) &&
                    /*(IssueType == null || i.IssueTitle == IssueType) &&*/
                    (Priority == null || i.Priority == Priority) &&
                    (assignType == null || i.IsAssigned == assignType) &&
                    (assign == null || i.Assignee == assign) &&
                    (Status == null || i.Status == Status) &&
                    (Createdby == null || i.CreatedBy == Createdby) &&
                    (issueid == 0 || i.IssueID == issueid) &&
                    (!fromDate.HasValue || fromDate <= i.CreatedOn) &&
                    (!toDate.HasValue || toDate >= i.CreatedOn)
                );

                // Execute the query and return the result
                var incidents = query.OrderBy(i => i.IssueID).ToList();
                return Json(incidents);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
