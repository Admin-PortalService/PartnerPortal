using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWebApplication.Data;
using TestWebApplication.Models.ViewModels;
using TestWebApplication.Common;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Mail;
using NuGet.Protocol.Plugins;
using TestWebApplication.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly ApplicationDbContext context;
        private UserManager<IdentityUser> userManager;

        public UsersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [RolePermissionAuthorize("Read")]
        public async Task<IActionResult> Index()
        {
            //ViewBag.ListCount = await context.Users.Where(t => t.UserName != "SuperAdmin").CountAsync();
            ViewBag.ListCount = await context.Users.CountAsync();
            if (TempData.ContainsKey("UserSuccess"))
            {
                ViewBag.msg = TempData["UserSuccess"];
            }
            return View();
        }

        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.RoleList = await context.Roles.Where(t => t.RoleID != Guid.Empty && t.RoleType != "SuperAdmin").OrderBy(t => t.RoleType).ToListAsync();
            //ViewBag.ProjectList = await context.Project.Where(t => t.ProjectID != 0).OrderBy(t => t.ProjectDesc).ToListAsync();
            ViewBag.LevelList = await context.Severity.Where(t => t.Level != null).ToListAsync();
            return View();
        }

        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> Create(Users model, List<string> RoleID)
        {
            string Name = User.Identity?.Name;
            var LoginName = context.UserAccess.Where(t => t.UserMail == Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                IdentityUser identityUser = new IdentityUser() { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
                IdentityResult result = await userManager.CreateAsync(identityUser, "P@ssw0rd1");
                                
                var first = model.FirstName;
                var last = model.LastName;

                if (result.Succeeded)
                {
                    //Added to UserProfile
                    UserProfile profile = new()
                    {
                        Id = identityUser.Id,
                        Email = identityUser.Email,
                        FirstName = first,
                        LastName = last,
                        JobTitle = "",
                        Phone = "",
                        Address = "",
                        City = "",
                        State = "",
                        Country = ""
                    };

                    context.UserProfile.Add(profile);

                    Users obj = new()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        ActiveFr = model.ActiveFr,
                        ActiveTo = model.ActiveTo,
                        IsActive = model.IsActive,
                        CreatedBy = LoginName.UserName,
                        ModifiedBy = LoginName.UserName,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        Levels = model.Levels,
                    };

                    List<IdentityUserRole<string>> identityUserRoles = new List<IdentityUserRole<string>>();
                    foreach (string roleIdValue in RoleID)
                    {
                        identityUserRoles.Add(new IdentityUserRole<string>() { UserId = identityUser.Id, RoleId = roleIdValue });
                    }

                    context.Users.Add(obj);
                    context.SaveChanges();
                    context.UserRoles.AddRange(identityUserRoles);
                    context.SaveChanges();

                    foreach (string roleId in RoleID)
                    {
                        var objRole = context.Roles.SingleOrDefault(m => m.RoleID.ToString() == roleId);
                        UserAccess UserAccess = new()
                        {
                            UserID = obj.UserID,
                            UserName = obj.FirstName + " " + obj.LastName,
                            UserDesc = obj.FirstName + " " + obj.LastName,
                            UserMail = obj.Email,
                            RoleName = objRole.RoleType,
                            StartDate = obj.ActiveFr,
                            EndDate = obj.ActiveTo,
                            CreatedBy = obj.CreatedBy,
                            ModifiedBy = obj.ModifiedBy,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                        };
                        context.UserAccess.Add(UserAccess);
                    }
                    context.SaveChanges();
                    var to = new List<string>();
                    var cc = new List<string>();
                    to.Add(model.Email);

                    //string body = "Welcome To Cuscen Portal.@Hello!@Your Cuscen Portal Account has been created.@Please login using the following credentials: @@ UserName : @Password : P@ssw0rd1";
                    //body = body.Replace("@", System.Environment.NewLine);
                    //body = body.Replace("@", "@\n");
                    SendNotificationtoUser("Portal Service", "Cuscen Portal Account Create Successful", to, cc, "Your Cuscen Portal Account has been created. Default Password is P@ssw0rd1");
                     
                    TempData["mydata"] = obj.UserID;
                    return RedirectToAction("Create", "UserAccess", new { Id = obj.UserID });
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("UserTable", error.Description);
                    }

                    var errors = new Dictionary<string, IEnumerable<string>>();
                    List<string> message = new List<string>();
                    foreach (var keyValue in ModelState)
                    {
                        if (keyValue.Value.Errors.Count > 0)
                        {
                            errors[keyValue.Key] = keyValue.Value.Errors.Select(e => e.ErrorMessage);//ValidationState
                            var msg1 = keyValue.Key;
                            var msg2 = string.Join(", ", errors["UserTable"]); //keyValue.Value.ValidationState;
                            string msg = msg1 + new string(' ', 3) + " : " + new string(' ', 3) + msg2;
                            message.Add(msg);
                        }

                    }
                    var strings = (from o in message
                                   select o.ToString()).ToList();
                    ViewBag.msgList = strings;
                    //TempData["msg"] = message;
                    //ModelState.Clear();
                    ViewBag.RoleList = await context.Roles.Where(t => t.RoleID != Guid.Empty).OrderBy(t => t.RoleType).ToListAsync();
                    //ViewBag.ProjectList = await context.Project.Where(t => t.ProjectID != 0).OrderBy(t => t.ProjectDesc).ToListAsync();
                    ViewBag.LevelList = await context.Severity.Where(t => t.Level != "").ToListAsync();
                    return View(model);
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
                //TempData["msg"] = message;
                //ModelState.Clear();
                ViewBag.RoleList = await context.Roles.Where(t => t.RoleID != Guid.Empty && t.RoleType != "SuperAdmin").OrderBy(t => t.RoleType).ToListAsync();
                //ViewBag.ProjectList = await context.Project.Where(t => t.ProjectID != 0).OrderBy(t => t.ProjectDesc).ToListAsync();
                ViewBag.LevelList = await context.Severity.Where(t => t.Level != "").ToListAsync();
                return View(model);
            }
        }
        public bool SendNotificationtoUser(string name, string subject, List<string> toList, List<string> ccList, string body)
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

        [HttpPost]
        public async Task<IActionResult> _ListDataPartial(string sortOrder, string searchType, string searchString, int firstItem = 0)
        {
            //Sort and filter test data
            IEnumerable<Users> query;
            if (sortOrder.ToLower(CultureInfo.InvariantCulture) == "descending")
            {
                //query = context.Users.Where(m => m.UserName != "SuperAdmin").OrderByDescending(m => m.UserName).AsEnumerable();
                query = context.Users.Where(m => m.FirstName != "SuperAdmin").OrderByDescending(m => m.FirstName).AsEnumerable();
            }
            else
            {
                //query = context.Users.Where(m => m.UserName != "SuperAdmin").OrderBy(m => m.UserName);
                query = context.Users.Where(m => m.FirstName != "SuperAdmin").OrderBy(m => m.FirstName);
            }
            if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchString))
            {
                if (searchType.Equals(SearchTypeConstant.CONTAIN, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    //(m.UserName is not null && m.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    (m.FirstName is not null && m.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                }
                else if (searchType.Equals(SearchTypeConstant.STARTWITH, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    //(m.UserName is not null && m.UserName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    (m.FirstName is not null && m.FirstName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                }
                else if (searchType.Equals(SearchTypeConstant.ENDWITH, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    //(m.UserName is not null && m.UserName.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    (m.FirstName is not null && m.FirstName.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                }
                else if (searchType.Equals(SearchTypeConstant.EQUAL, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    //(m.UserName is not null && m.UserName.Equals(searchString, StringComparison.OrdinalIgnoreCase)));
                    (m.FirstName is not null && m.FirstName.Equals(searchString, StringComparison.OrdinalIgnoreCase)));
                }
            }
            // Extract a portion of data
            List<Users> model = query.Skip(firstItem).Take(BATCHSIZE).ToList();
            if (model.Count == 0)
            {
                return StatusCode(204); // 204 := "No Content"
            }

            return PartialView(model);

        }

        [HttpGet]
        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(int Id)
        {

            var objUser = context.Users.SingleOrDefault(m => m.UserID == Id);
            //ViewBag.RoleDesc = context.Roles.Where(t => t.RoleID == objUser.RoleID).ToList();
            //ViewBag.ProjectDesc = context.Project.Where(t => t.ProjectID == objUser.ProjectID).ToList();
            var result = new Users()
            {
                UserID = objUser.UserID,
                FirstName = objUser.FirstName,
                LastName = objUser.LastName,
                Email = objUser.Email,
                CreatedBy = objUser.CreatedBy,
                CreatedOn = objUser.CreatedOn,
                ModifiedBy = objUser.ModifiedBy,
                ModifiedOn = DateTime.Now,
                ActiveFr = objUser.ActiveFr,
                ActiveTo = objUser.ActiveTo,
                IsActive = objUser.IsActive
            };

            var roleList = context.Roles.Where(t => t.RoleID != Guid.Empty).OrderBy(t => t.RoleType).ToList();
            var user = userManager.FindByEmailAsync(objUser.Email).Result;
            var userRoleIdList = context.UserRoles.Where(r => r.UserId == user.Id);

            SelectList roleSelectList = new SelectList(roleList, "RoleID", "RoleType");
            roleSelectList.Where(r => userRoleIdList.Any(u => u.RoleId == r.Value)).ToList().ForEach(r => r.Selected = true);

            ViewBag.Roles = roleSelectList;

            ViewBag.RoleList = context.UserAccess.SingleOrDefault(r => r.UserID == Id);

            ViewBag.userLevelList = context.Severity.Where(t => t.Level != "").ToList();
            ViewBag.Level = context.Users.SingleOrDefault(u => u.UserID == Id);

            return View(result);

        }

        [HttpPost]
        [RolePermissionAuthorize("Update")]
        public async Task<IActionResult> Edit(Users model, List<string> RoleID)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.FindByEmailAsync(model.Email).Result;
                List<IdentityUserRole<string>> identityUserRoles = new List<IdentityUserRole<string>>();
                if (RoleID.Count != 0)
                {
                    foreach (string roleIdValue in RoleID)
                    {
                        identityUserRoles.Add(new IdentityUserRole<string>() { UserId = user.Id, RoleId = roleIdValue });
                    }
                    context.UserRoles.RemoveRange(context.UserRoles.Where(u => u.UserId == user.Id));
                    context.UserRoles.AddRange(identityUserRoles);
                }
                

                context.Users.Update(model);
                var UserAcc = context.UserAccess.SingleOrDefault(m => m.UserID == model.UserID);
                UserAcc.UserName = model.FirstName + " " + model.LastName;
                context.UserAccess.Update(UserAcc);
                
                context.SaveChanges();

                TempData["mydata"] = model.UserID;

                return RedirectToAction("Edit", "UserAccess", new { Id = model.UserID });
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
                ViewBag.RoleList = await context.Roles.Where(t => t.RoleID != Guid.Empty).OrderBy(t => t.RoleType).ToListAsync();
                ViewBag.ProjectList = await context.Project.Where(t => t.ProjectID != 0).OrderBy(t => t.ProjectDesc).ToListAsync();
                return View(model);

            }
        }
        [RolePermissionAuthorize("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int UserID)
        {
            if (context.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users' is null.");
            }
            var users = await context.Users.FindAsync(UserID);
            if (users != null)
            {
                context.Users.Remove(users);
            }
            var UserAcc = context.UserAccess.Where(m => m.UserID == UserID).FirstOrDefault();
            if (UserAcc != null)
            {
                context.UserAccess.Remove(UserAcc);
            }
            var user = userManager.FindByEmailAsync(users.Email);
            if (user != null)
            {
                await userManager.DeleteAsync(user.Result);
            }
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }       
    }
}