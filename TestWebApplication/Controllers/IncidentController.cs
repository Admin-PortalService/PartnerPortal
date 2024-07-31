using Microsoft.AspNetCore.Mvc;
using TestWebApplication.Data;
using TestWebApplication.Models.ViewModels;
using TestWebApplication.Common;
using Microsoft.EntityFrameworkCore;
using TestWebApplication.Helpers;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class IncidentController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads");
        private IWebHostEnvironment Environment;
        private readonly Microsoft.AspNetCore.Http.IFormFile file;
        private static readonly string[] AllowedExtensions = { ".xls", ".xlsx", ".doc", ".docx", ".ppt", ".pptx", ".pdf", ".zip", ".txt", ".png", ".jpg", ".jpeg" };

        //CustomIDataProtection customIDataProtection) {  
        // protector = customIDataProtection; 
        public IncidentController(ApplicationDbContext context, IWebHostEnvironment _environment)
        {
            this.context = context;
            Environment = _environment;
            //protector = dataProtectionProvider.CreateProtector(uniqueCode.IdRouteValue);
        }
        [RolePermissionAuthorize("Read")]
        public async Task<IActionResult> IndexIncident()
        {
            try
            {
                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userName = User.Identity?.Name;
                var incident = new List<Incident>();
                //var Role = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();
                if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    ViewBag.IncidentList = context.Incident.Where(t => t.IssueID != 0).OrderByDescending(t => t.CreatedOn).ToList();
                    ViewBag.ModuleList = context.Module.Where(t => t.ModuleID != 0).OrderBy(t => t.ModuleName).ToList();
                    ViewBag.ProjectList = context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToList();
                    ViewBag.IssueList = context.IssueType.Where(i => i.IssueTypeID != 0).OrderBy(i => i.IssueTypeName).ToList();
                    ViewBag.PriorityList = context.Severity.Where(i => i.Priority != "" && i.Priority != null).OrderBy(i => i.No).ToList();
                    ViewBag.StatusList = context.Severity.OrderBy(i => i.No).ToList();
                    ViewBag.UserList = context.UserAccess.Where(u => u.UserID != 0).OrderBy(i => i.UserName).ToList();
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

                    foreach (var prjId in projectIDs)
                    {
                        var incidentList = context.Incident.Where(t => t.IssueID != 0 && t.ProjectID == prjId.ProjectID).OrderByDescending(t => t.CreatedOn).ToList();
                        incident.AddRange(incidentList);
                    }
                    ViewBag.IncidentList = incident;
                    ViewBag.ModuleList = context.Module.Where(t => t.ModuleID != 0).OrderBy(t => t.ModuleName).ToList();
                    ViewBag.ProjectList = context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToList();
                    ViewBag.IssueList = context.IssueType.Where(i => i.IssueTypeID != 0).OrderBy(i => i.IssueTypeName).ToList();
                    ViewBag.PriorityList = context.Severity.Where(i => i.Priority != "" && i.Priority != null).OrderBy(i => i.No).ToList();
                    ViewBag.StatusList = context.Severity.OrderBy(i => i.No).ToList();
                    ViewBag.UserList = context.UserAccess.Where(u => u.UserID != 0).OrderBy(i => i.UserName).ToList();

                }
                //var result = ViewBag.IncidentList;
                ViewBag.module = context.Module.ToList();
                ViewBag.project = context.Project.ToList();
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
                ViewBag.IssueType = await context.IssueType.Where(t => t.IssueTypeID != 0).OrderBy(t => t.IssueTypeDesc).ToListAsync();
                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userName = User.Identity?.Name;
                var module = new List<Module>();
                var customerID = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();
                //var Role = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();

                if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    ViewBag.ProjectList = await context.Project.OrderBy(t => t.ProjectDesc).ToListAsync();
                    ViewBag.ModuleList = await context.Module.Where(t => t.ModuleID != 0).OrderBy(t => t.ModuleDesc).ToListAsync();

                }
                else
                {
                    var userID = context.UserAccess.Where(t => t.UserMail == userName).Select(t => t.UserID).FirstOrDefault();
                    var customerIDs = context.User_Customer.Where(u => u.UserID == userID).ToList();

                    var projectIDs = new List<Projects>();
                    foreach (var cusId in customerIDs)
                    {
                        var projectList = await context.Project.Where(t => t.CustomerID == cusId.CustomerID).ToListAsync();
                        projectIDs.AddRange(projectList);
                    }

                    foreach (var projectID in projectIDs)
                    {
                        var moduleList = await context.Module.Where(i => i.ModuleID != 0 && i.ProjectID == projectID.ProjectID).ToListAsync();
                        module.AddRange(moduleList);
                    }

                    ViewBag.ProjectList = projectIDs;
                    ViewBag.ModuleList = module;
                }

                ViewBag.Priority = await context.Severity.Where(t => t.Priority != null).ToListAsync();
                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> Create(Incident IncModel, Attachment atc, IFormFileCollection UploadFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = HttpContext.User;
                    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    string userName = User.Identity?.Name;
                    var ReporterName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                    if (IncModel.Priority == "Please Select Priority")
                    {
                        ViewBag.errormsg = "Please select Priority";
                        ViewBag.ProjectList = await context.Project.Where(t => t.ProjectID != 0).OrderBy(t => t.ProjectDesc).ToListAsync();
                        ViewBag.ModuleList = await context.Module.Where(t => t.ModuleID != 0).OrderBy(t => t.ModuleDesc).ToListAsync();
                        ViewBag.Priority = await context.Severity.Where(t => t.Priority != "").ToListAsync();
                        ViewBag.IssueType = await context.IssueType.Where(t => t.IssueTypeID != 0).OrderBy(t => t.IssueTypeDesc).ToListAsync();
                        return View();
                    }
                    if (IncModel.Category == "Please Select Issue Category")
                    {
                        ViewBag.errormsg = "Please select Issue Category";
                        ViewBag.ProjectList = await context.Project.Where(t => t.ProjectID != 0).OrderBy(t => t.ProjectDesc).ToListAsync();
                        ViewBag.ModuleList = await context.Module.Where(t => t.ModuleID != 0).OrderBy(t => t.ModuleDesc).ToListAsync();
                        ViewBag.Priority = await context.Severity.Where(t => t.Priority != "").ToListAsync();
                        ViewBag.IssueType = await context.IssueType.Where(t => t.IssueTypeID != 0).OrderBy(t => t.IssueTypeDesc).ToListAsync();
                        return View();
                    }

                    var sla = context.SLAConfig.Where(t => t.Priority == IncModel.Priority).FirstOrDefault();
                    DateTime ActualBreachedTime = DateTime.Now.AddMinutes(sla.SLAmin);

                    Incident objInc = new()
                    {
                        //IssueID = generateId(),
                        IssueTitle = IncModel.IssueTitle,
                        Category = IncModel.Category,
                        ProjectID = IncModel.ProjectID,
                        ModuleID = IncModel.ModuleID,
                        Priority = IncModel.Priority,
                        AreaPath = IncModel.AreaPath,
                        DetailDesc = IncModel.DetailDesc,
                        Status = IncModel.Status,
                        CreatedBy = ReporterName.UserName,
                        ModifiedBy = ReporterName.UserName,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        AltContact = IncModel.AltContact,//Add Alternate contact
                        AltEmail = IncModel.AltEmail,//Add Alternate Email
                        AltPhone = IncModel.AltPhone, //Ass Alternate Phone
                        ActualDate = ActualBreachedTime
                    };
                    context.Incident.Add(objInc);
                    context.SaveChanges();

                    //Added Incident History
                    var status = context.Severity.Where(t => t.IncidentStatus == IncModel.Status).Select(t => t.IncidentStatus).FirstOrDefault();
                    var priority = context.Severity.Where(t => t.Priority == IncModel.Priority).Select(t => t.Priority).FirstOrDefault();
                    IncidentHistory history = new()
                    {
                        IssueID = objInc.IssueID,
                        Status = status,
                        Priority = priority,
                        UserName = userName,
                        CreatedOn = DateTime.Now
                    };
                    context.IncidentHistory.Add(history);
                    context.SaveChanges();

                    if (IncModel.UploadFile != null)
                    {
                        DateTime todayDate = DateTime.Now;

                        foreach (var files in UploadFile)
                        {
                            using (var target = new MemoryStream())
                            {
                                Attachment objAttach = new Attachment();
                                files.CopyTo(target);
                                objAttach.Content = target.ToArray();
                                objAttach.IssueID = objInc.IssueID;
                                objAttach.AttachName = files.FileName;
                                objAttach.CreatedBy = objInc.CreatedBy;
                                objAttach.CreatedOn = todayDate;
                                context.Attachment.Update(objAttach);

                                context.SaveChanges();
                            }
                        }
                    }
                    //var Role = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                    List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();
                    var module = new List<Module>();
                    if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                    {
                        ViewBag.ProjectList = await context.Project.OrderBy(t => t.ProjectDesc).ToListAsync();
                        ViewBag.ModuleList = await context.Module.Where(t => t.ModuleID != 0).OrderBy(t => t.ModuleDesc).ToListAsync();
                    }
                    else
                    {
                        var userID = context.UserAccess.Where(t => t.UserMail == userName).Select(t => t.UserID).FirstOrDefault();
                        var customerIDs = context.User_Customer.Where(u => u.UserID == userID).ToList();

                        var projectIDs = new List<Projects>();
                        foreach (var cusId in customerIDs)
                        {
                            projectIDs = context.Project.Where(t => t.CustomerID == cusId.CustomerID).OrderBy(t => t.ProjectDesc).ToList();
                        }
                                               
                        foreach (var projectID in projectIDs)
                        {
                            var moduleList = await context.Module.Where(i => i.ModuleID != 0 && i.ProjectID == projectID.ProjectID).ToListAsync();
                            module.AddRange(moduleList);
                        }

                        ViewBag.ProjectList = projectIDs;
                        ViewBag.ModuleList = module;
                    }

                    ViewBag.Priority = await context.Severity.Where(t => t.Priority != null).ToListAsync();

                    ViewBag.IssueType = await context.IssueType.Where(t => t.IssueTypeID != 0).OrderBy(t => t.IssueTypeDesc).ToListAsync();

                    ViewBag.msg = "Incident Successfully Created";

                    TempData["IncCreateUsr"] = ReporterName.UserMail;

                    ModelState.Clear();
                    return View();

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
                    ViewBag.ProjectList = await context.Project.Where(t => t.ProjectID != 0).OrderBy(t => t.ProjectDesc).ToListAsync();
                    ViewBag.ModuleList = await context.Module.Where(t => t.ModuleID != 0).OrderBy(t => t.ModuleDesc).ToListAsync();
                    ViewBag.Priority = await context.Severity.Where(t => t.Priority != null).ToListAsync();
                    ViewBag.IssueType = await context.IssueType.Where(t => t.IssueTypeID != 0).OrderBy(t => t.IssueTypeDesc).ToListAsync();
                    var strings = (from o in message
                                   select o.ToString()).ToList();
                    ViewBag.msgList = strings;
                    //ModelState.Clear();
                    return View();
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpGet]
        [RolePermissionAuthorize("Update")]
        public IActionResult Tab(int Id)
        {
            try
            {                                
                //ViewBag.ID = Base64Encode(Id);
                //int IdDecode = Base64Decode(Id);
                ViewBag.Incident = context.Incident.Where(t => t.IssueID == Id).ToList();
                ViewBag.Comment = context.Comment.Where(t => t.IssueID == Id).OrderBy(t => t.CreatedOn).ToList();
                ViewBag.InternalNote = context.InternalNote.Where(t => t.IssueID == Id).OrderBy(t => t.CreatedOn).ToList();
                ViewBag.Solution = context.Solution.Where(t => t.IssueID == Id).OrderBy(t => t.CreatedOn).ToList();
                ViewBag.Attachment = context.Attachment.Where(t => t.IssueID == Id).OrderBy(t => t.CreatedOn).ToList();
                ViewBag.AttachName = context.Attachment.Where(t => t.IssueID == Id).OrderBy(t => t.CreatedOn).FirstOrDefault();
                ViewBag.IncidentHistory = context.IncidentHistory.Where(t => t.IssueID == Id).OrderBy(t => t.CreatedOn).ToList();

                var objInc = context.Incident.SingleOrDefault(m => m.IssueID == Id);
                var result = new Incident()
                {
                    ProjectID = objInc.ProjectID,
                    ModuleID = objInc.ModuleID,
                    Category = objInc.Category,
                    Status = objInc.Status,
                    Priority = objInc.Priority,
                    AltContact = objInc.AltContact,
                    AltEmail = objInc.AltEmail,
                    AltPhone = objInc.AltPhone,
                    PauseStart = objInc.PauseStart,
                    PauseEnd = objInc.PauseEnd,
                    PauseDuration = objInc.PauseDuration
                };
                ViewBag.hasBreached = objInc.HasBreached;
                ViewBag.IssueType = context.IssueType.SingleOrDefault(t => t.IssueTypeName == result.Category);
                ViewBag.IssueList = context.IssueType.Where(t => t.IssueTypeName != objInc.Category).OrderBy(t => t.IssueTypeName).ToList();
                ViewBag.ProjectName = context.Project.SingleOrDefault(t => t.ProjectID == result.ProjectID);

                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var userRoleId = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().SingleOrDefault();
                var isInternal = context.Roles.Where(t => t.RoleID.ToString() == userRoleId.ToUpper()).Select(t => t.IsInternal).Distinct().SingleOrDefault();
                ViewBag.IsInternal = isInternal;

                string userName = User.Identity?.Name;
                var module = new List<Module>();
                var customerID = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();
                //var Role = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();
                List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();

                if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    ViewBag.ProjectList = context.Project.OrderBy(t => t.ProjectDesc).ToList();
                    ViewBag.ModuleList = context.Module.Where(t => t.ModuleID != objInc.ModuleID).OrderBy(t => t.ModuleDesc).ToList();
                }
                else
                {
                    var userID = context.UserAccess.Where(t => t.UserMail == userName).Select(t => t.UserID).FirstOrDefault();
                    var customerIDs = context.User_Customer.Where(u => u.UserID == userID).ToList();

                    var projectIDs = new List<Projects>();
                    foreach (var cusId in customerIDs)
                    {
                        projectIDs = context.Project.Where(t => t.CustomerID == cusId.CustomerID).OrderBy(t => t.ProjectDesc).ToList();
                    }

                    foreach (var projectID in projectIDs)
                    {
                        var moduleList = context.Module.Where(i => i.ModuleID != 0 && i.ProjectID == projectID.ProjectID).ToList();
                        module.AddRange(moduleList);
                    }

                    ViewBag.ProjectList = projectIDs;
                    ViewBag.ModuleList = module;
                }
                ViewBag.ModuleName = context.Module.SingleOrDefault(t => t.ModuleID == result.ModuleID);
                ViewBag.Status = context.Severity.SingleOrDefault(t => t.IncidentStatus == result.Status);
                ViewBag.StatusList = context.Severity.Where(t => t.IncidentStatus != objInc.Status && t.IncidentStatus != "").ToList();
                ViewBag.Priority = context.Severity.SingleOrDefault(t => t.Priority == result.Priority);
                ViewBag.PriorityList = context.Severity.Where(t => t.Priority != objInc.Priority && t.Priority != null).ToList();

                string userN = User.Identity?.Name;
                ViewBag.IsReporter = context.UserAccess.Where(t => t.UserMail == userN && t.RoleName.Contains("Report")).Count();

                if (TempData["Message"] != null)
                {
                    ViewBag.msg = "Incident Successfully Updated.";
                }

                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpPost]
        [RolePermissionAuthorize("Update")]
        public async Task<IActionResult> Tab(Incident inc, IFormFileCollection UploadFile)
        {
            try
            {
                string userName = User.Identity?.Name;

                //Added Incident History
                var status = context.Severity.Where(t => t.IncidentStatus == inc.Status).Select(t => t.IncidentStatus).FirstOrDefault();
                var priority = context.Severity.Where(t => t.Priority == inc.Priority).Select(t => t.Priority).FirstOrDefault();
                var modified = context.IncidentHistory.Where(t => t.IssueID == inc.IssueID && t.ModifiedOn == null).FirstOrDefault(); 
                var incHis = context.IncidentHistory.Where(t => t.IssueID == inc.IssueID).OrderByDescending(t => t.CreatedOn).FirstOrDefault();

                if (modified != null)
                {
                    modified.ModifiedOn = DateTime.Now;
                    context.IncidentHistory.Update(modified);
                }
                IncidentHistory history = new()
                {
                    IssueID = inc.IssueID,
                    //StatusID = status.No,
                    Status = status,
                    Priority = priority,
                    UserName = userName,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = (inc.Status == "Closed") ? DateTime.Now : null
                };
                context.IncidentHistory.Add(history);
                context.SaveChanges();

                //HasBreached && ActualBreachedTime && SLATime
                bool hasBreached = false; DateTime? ActualBreachedTime = null; TimeSpan? SLADate = new TimeSpan(0, 0, 0, 0); string SLATime = "";
                DateTime? pauseEnd = null;
                string pauseDuration = ""; string totalPauseDuration = "";

                hasBreached = (inc.ActualDate < DateTime.Now) ? true : false;
                ViewBag.hasBreached = hasBreached;

                switch (inc.Status, hasBreached, incHis.Status)
                {
                    case ("Pause", true, _):
                        inc.PauseStart = DateTime.Now;
                        SLADate = DateTime.Now - inc.ActualDate;
                        break;
                    case ("Pause", false, _):
                        inc.PauseStart = DateTime.Now;
                        break;
                    case (not "Pause", true, "Pause"):
                        SLADate = DateTime.Now - inc.ActualDate;
                        pauseEnd = DateTime.Now;
                        break;
                    case (not "Pause", false, "Pause"):
                        pauseEnd = DateTime.Now;
                        break;
                    case (not "Pause", true, _):
                        SLADate = DateTime.Now - inc.ActualDate;
                        break;
                    default:
                        break;
                }

                if (SLADate.HasValue)
                {
                    TimeSpan slaDate = SLADate.Value;
                    SLATime = string.Format("{0} D : {1} hr : {2} min : {3} sec", slaDate.Days, slaDate.Hours, slaDate.Minutes, slaDate.Seconds);
                }


                if (pauseEnd != null)
                {
                    TimeSpan? ts = pauseEnd - inc.PauseStart;
                    if (ts.HasValue)
                    {
                        TimeSpan timespan = ts.Value;

                        if (inc.PauseDuration != null)
                        {
                            pauseDuration = string.Format("{0} min : {1} sec", timespan.Minutes, timespan.Seconds);
                            totalPauseDuration = SumTimeString(pauseDuration, inc.PauseDuration);
                            inc.PauseDuration = totalPauseDuration;
                        }
                        else
                        {
                            inc.PauseDuration = string.Format("{0} min : {1} sec", timespan.Minutes, timespan.Seconds);
                        }

                        ActualBreachedTime = (inc.ActualDate).Value.AddMinutes(timespan.TotalMinutes);
                    }
                }

                var ReporterName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();
                var objInc = new Incident()
                {
                    IssueID = inc.IssueID,
                    IssueTitle = inc.IssueTitle,
                    Category = inc.Category,
                    ProjectID = inc.ProjectID,
                    ModuleID = inc.ModuleID,
                    AreaPath = inc.AreaPath,
                    Priority = inc.Priority,
                    DetailDesc = inc.DetailDesc,
                    Status = inc.Status,
                    UploadFile = inc.UploadFile,
                    CreatedBy = inc.CreatedBy,
                    CreatedOn = inc.CreatedOn,
                    ModifiedBy = ReporterName.UserName,
                    ModifiedOn = DateTime.Now,
                    Assignee = inc.Assignee,
                    IsAssigned = inc.IsAssigned,
                    AltContact = inc.AltContact,
                    AltEmail = inc.AltEmail,
                    AltPhone = inc.AltPhone,
                    HasBreached = hasBreached,
                    ActualDate = (ActualBreachedTime != null) ? ActualBreachedTime : inc.ActualDate,
                    SLADate = SLATime,
                    PauseStart = inc.PauseStart, 
                    PauseEnd = pauseEnd,
                    PauseDuration = inc.PauseDuration 
                };
                context.Incident.Update(objInc);
                context.SaveChanges();

                ViewBag.Priority = context.Severity.SingleOrDefault(t => t.Priority == objInc.Priority);

                TempData["Message"] = "Incident Successfully Updated.";
                return RedirectToAction("Tab", "Incident", new { @id = objInc.IssueID });
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [RolePermissionAuthorize("Delete")]
        public IActionResult Delete(int Id)
        {
            try
            {
                var objInc = context.Incident.SingleOrDefault(m => m.IssueID == Id);
                context.Incident.Remove(objInc);
                context.SaveChanges();
                return RedirectToAction("IndexIncident");
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<JsonResult> CheckUploadDocumentFile()
        {
            try
            {
                Dictionary<string, string> dt1 = new Dictionary<string, string>();
                string directory = string.Empty, filePathAndName = string.Empty;
                List<string> uploadedFiles = new List<string>();

                if (Request.Form?.Files != null && Request.Form?.Files.Count != 0)
                {
                    var files = Request.Form?.Files;
                    IFormFile file = files[0];
                    if (files.Count > 1)
                    {
                        dt1.Add(UploadConstants.MessageKey, "TooManyFiles");
                        return Json(dt1);
                    }
                    else if (file.Length == 0)
                    {
                        dt1.Add(UploadConstants.MessageKey, "EmptyFile");
                        return Json(dt1);
                    }
                    else if (!(file.FileName.EndsWith(".xlsx") || file.FileName.EndsWith(".xls")
                            || file.FileName.EndsWith(".docx") || file.FileName.EndsWith(".doc")
                            || file.FileName.EndsWith(".pptx") || file.FileName.EndsWith(".ppt")
                            || file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".zip") || file.FileName.EndsWith(".txt")
                            || file.FileName.EndsWith(".jpg") || file.FileName.EndsWith(".jpeg") || file.FileName.EndsWith(".png")))
                    {
                        dt1.Add(UploadConstants.MessageKey, "InvalidFileExtension");
                        return Json(dt1);
                    }
                    else if (file.Length > 20971520)
                    {
                        dt1.Add(UploadConstants.MessageKey, "FileSize");
                        return Json(dt1);
                    }
                    else
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        dt1.Add(UploadConstants.MessageKey, "Success");
                        return Json(dt1);
                    }
                }
                else
                {
                    dt1.Add(UploadConstants.MessageKey, "EmptyFile");
                    return Json(dt1);
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dt = new Dictionary<string, string>();
                dt.Add(UploadConstants.MessageKey, "Error");
                //dt.Add(UploadConstants.ErrorMessage, "Server Error on Uploading File.");
                dt.Add(UploadConstants.ErrorMessage, ex.Message);
                return Json(dt);
            }
        }

        //public async Task<IActionResult> DetailCmt(int Id)
        //{
        //    //ViewBag.ID = Id;
        //    //int IdDecode = Base64Decode(Id);           
        //    ViewBag.Incident = await context.Incident.Where(t => t.IssueID == Id).ToListAsync();
        //    ViewBag.Comment = await context.Comment.Where(t => t.IssueID == Id).OrderBy(t => t.CreatedOn).ToListAsync();
        //    ViewBag.Attachment = await context.Attachment.Where(t => t.IssueID == Id).OrderBy(t => t.CreatedOn).ToListAsync();
            
        //    return View();
        //    //return RedirectToAction("DetailCmt", "Incident", new { @id = Id });
        //}


        public async Task<IActionResult> Comment(Comment cmtModel)
        {
            try
            {
                string userName = User.Identity?.Name;
                var ReporterName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    Comment obj = new()
                    {
                        Subject = cmtModel.Subject,
                        CmtMsg = cmtModel.CmtMsg,
                        IssueID = cmtModel.IssueID,
                        CreatedBy = ReporterName.UserName,
                        CreatedOn = DateTime.Now,
                    };

                    context.Comment.Add(obj);
                    context.SaveChanges();
                    return RedirectToAction("Tab", "Incident", new { @id = obj.IssueID });

                }
                ModelState.Clear();
                return RedirectToAction("Tab", "Incident", new { @id = cmtModel.IssueID });
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public async Task<IActionResult> InternalNote(InternalNote noteModel)
        {
            try
            {
                string userName = User.Identity?.Name;
                var ReporterName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    InternalNote obj = new()
                    {
                        Subject = noteModel.Subject,
                        Note = noteModel.Note,
                        IssueID = noteModel.IssueID,
                        CreatedBy = ReporterName.UserName,
                        CreatedOn = DateTime.Now,
                    };

                    context.InternalNote.Add(obj);
                    context.SaveChanges();
                    return RedirectToAction("Tab", "Incident", new { @id = obj.IssueID });

                }
                ModelState.Clear();
                return RedirectToAction("Tab", "Incident", new { @id = noteModel.IssueID });
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public async Task<IActionResult> Solution(Solution solutionMdl)
        {
            try
            {
                string userName = User.Identity?.Name;
                var ReporterName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    Solution Solobj = new()
                    {
                        SolTitle = solutionMdl.SolTitle,
                        SolDesc = solutionMdl.SolDesc,
                        Note = solutionMdl.Note,
                        IssueID = solutionMdl.IssueID,
                        CreatedBy = ReporterName.UserName,
                        CreatedOn = DateTime.Now,
                    };

                    context.Solution.Add(Solobj);
                    context.SaveChanges();
                    return RedirectToAction("Tab", "Incident", new { @id = Solobj.IssueID });

                }
                ModelState.Clear();
                return RedirectToAction("Tab", "Incident", new { @id = solutionMdl.IssueID });
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public ActionResult DownloadFile(int fileID = 0)
        {
            try
            {
                if (fileID != 0)
                {
                    Attachment attchmentModel = context.Attachment.Where(m => m.AttachID == fileID).FirstOrDefault();

                    if (attchmentModel != null)
                    {
                        return File(attchmentModel.Content, System.Net.Mime.MediaTypeNames.Application.Octet, attchmentModel.AttachName);
                    }
                }
                return RedirectToAction("AccessDenied", "Error");
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public ActionResult AddMorePartialView()
        {
            AddMoreViewModel model = new AddMoreViewModel();
            return PartialView("_AddMorePartialView", model);
        } 

        //[HttpPost]
        //public ActionResult PostAddMore(AddMoreViewModel model)
        //{
        //    return View();
        //}

        private bool IsFileExtensionAllowed(string fileName)
        {
            return AllowedExtensions.Any(ext => fileName.EndsWith(ext));
        }

        [HttpPost]
        public async Task<IActionResult> Attach(Incident inc,Attachment atc, IFormFileCollection UploadFile)
        {

            try
            {
                if (inc.UploadFile != null)
                {
                    ViewBag.Attachment = await context.Attachment.Where(t => t.IssueID == inc.IssueID).OrderBy(t => t.CreatedOn).ToListAsync();

                    string userName = User.Identity?.Name;
                    var ReporterName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();
                    DateTime todayDate = DateTime.Now;
                    foreach (var files in UploadFile)
                    {
                        using (var target = new MemoryStream())
                        {
                            if (files.Length > 20971520)
                            {
                                Dictionary<string, string> dt2 = new Dictionary<string, string>();
                                dt2.Add(UploadConstants.MessageKey, "File size is too large. Allowed Max FileSize (20MB).");
                                return Json(dt2);
                            }

                            else if (!IsFileExtensionAllowed(files.FileName))
                            {
                                return BadRequest("Invalid file type");
                            }

                            else
                            {
                                Attachment objAttach = new Attachment();
                                files.CopyTo(target);
                                objAttach.Content = target.ToArray();
                                objAttach.IssueID = inc.IssueID;
                                objAttach.AttachName = files.FileName;
                                objAttach.CreatedBy = ReporterName.UserName;
                                objAttach.CreatedOn = todayDate;

                                context.Attachment.Update(objAttach);
                                context.SaveChanges();
                            }
                        }
                    }
                    return RedirectToAction("Tab", "Incident", new { @id = inc.IssueID });
                }

                return RedirectToAction("Tab", "Incident", new { @id = inc.IssueID });
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public int generateId()
        {
            //int id = context.Incident.Select(d => d.IssueID).DefaultIfEmpty(0).Max();
            int id = context.Incident.Max(p => (int?)p.IssueID) ?? 10000;
            return id += 1;

        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static int Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            int IDDecode = int.Parse(System.Text.Encoding.UTF8.GetString(base64EncodedBytes));
            return IDDecode;
        }

        [HttpPost]
        public JsonResult GetIncidentList(int ModuleId, int PrjId, string Issue, string Priority, string Status, string Createdby, DateTime FromDate, DateTime ToDate, int IssueId)
        {
            try
            {
                var incident = context.Incident.Where(i => (i.ProjectID == PrjId || PrjId == 0) && (i.ModuleID == ModuleId || ModuleId == 0) && (i.IssueTitle == Issue || Issue == null) &&
                                  (i.Priority == Priority || Priority == null) && (i.Status == Status || Status == null) && (i.CreatedBy == Createdby || Createdby == null) && (i.IssueID == IssueId || IssueId == 0)
                                  || (FromDate <= i.CreatedOn) && (ToDate >= i.CreatedOn)).OrderBy(i => i.IssueID).ToList();

                var project = context.Project.ToList();
                var module = context.Module.ToList();

                var result = new
                {
                    IncidentList = incident,
                    ProjectData = project,
                    ModuleData = module
                };

                return Json(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string SumTimeString(string t1, string t2)
        {
            // Extract numbers from the time strings using regular expressions
            var regex = new Regex(@"(\d+) min : (\d+) sec");

            var match1 = regex.Match(t1);
            var match2 = regex.Match(t2);

            int minutes1 = int.Parse(match1.Groups[1].Value);
            int seconds1 = int.Parse(match1.Groups[2].Value);

            int minutes2 = int.Parse(match2.Groups[1].Value);
            int seconds2 = int.Parse(match2.Groups[2].Value);

            int totalMin = minutes1 + minutes2;
            int totalSec = seconds1 + seconds2;

            totalMin += totalSec / 60;
            totalSec %= 60;

            return $"{totalMin} min : {totalSec} sec";
        }
    }
}
