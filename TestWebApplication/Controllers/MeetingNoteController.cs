using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class MeetingNoteController : BaseController
    {
        private readonly ApplicationDbContext context;
        public MeetingNoteController(ApplicationDbContext context)
        {
            this.context = context;
        }
        
        [RolePermissionAuthorize("Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0).OrderBy(t => t.CustomerName).ToListAsync();
                ViewBag.ProjectList = await context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToListAsync();

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
                ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0).OrderBy(t => t.CustomerName).ToListAsync();
                ViewBag.ProjectList = await context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToListAsync();
                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> Create(MeetingNote meetingNote)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MeetingNote obj = new()
                    {
                        Organization = meetingNote.Organization,
                        Project = meetingNote.Project,
                        Date = meetingNote.Date,
                        Title = meetingNote.Title,
                        Note = meetingNote.Note,
                        Attendance = meetingNote.Attendance
                    };
                    context.MeetingNote.Add(obj);
                    context.SaveChanges();
                    TempData["Success"] = "New Meeting Note is successfully noted!";

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
                    ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0).OrderBy(t => t.CustomerName).ToListAsync();
                    ViewBag.ProjectList = await context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToListAsync();
                    return View(meetingNote);

                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public JsonResult GetProjectsByOrgId(int orgId)
        {
            try
            {
                List<Projects> projectList = new List<Projects>();
                if (orgId != null)
                    projectList = context.Project.Where(p => p.CustomerID == orgId).ToList();

                return Json(projectList);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public IActionResult NoteList(int organization, int project)
        {
            try
            {
                List<MeetingNote> meetingNoteList = new List<MeetingNote>();
                if (organization != 0 || project != 0)
                    meetingNoteList = context.MeetingNote.Where(m => m.Organization == organization || m.Project == project).ToList();

                return PartialView("_GridView", meetingNoteList);
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
                var obj = context.MeetingNote.SingleOrDefault(m => m.MeetingNoteId == Id);
                var result = new MeetingNote()
                {
                    MeetingNoteId = obj.MeetingNoteId,
                    Organization = obj.Organization,
                    Project = obj.Project,
                    Title = obj.Title,
                    Note = obj.Note,
                    Date = obj.Date,
                    Attendance = obj.Attendance
                };
                ViewBag.CustomerList = context.Customer.Where(t => t.CustomerID != 0).OrderBy(t => t.CustomerName).ToList();
                ViewBag.ProjectList = context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToList();

                return View(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(MeetingNote meetingNote)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var obj = new MeetingNote()
                    {
                        MeetingNoteId = meetingNote.MeetingNoteId,
                        Organization = meetingNote.Organization,
                        Project = meetingNote.Project,
                        Title = meetingNote.Title,
                        Note = meetingNote.Note,
                        Date = meetingNote.Date,
                        Attendance = meetingNote.Attendance
                    };
                    context.MeetingNote.Update(obj);
                    context.SaveChanges();
                    TempData["Success"] = "Meeting Note is successfully modified!";
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
                    ViewBag.CustomerList = context.Customer.Where(t => t.CustomerID != 0).OrderBy(t => t.CustomerName).ToList();
                    ViewBag.ProjectList = context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToList();
                    return View(meetingNote);

                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
