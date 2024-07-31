using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Globalization;
using TestWebApplication.Data;
using TestWebApplication.Models.ViewModels;
using TestWebApplication.Common;
using Microsoft.EntityFrameworkCore;
using TestWebApplication.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class UserAccessController : BaseController
    {
        private readonly ApplicationDbContext context;
        public UserAccessController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [RolePermissionAuthorize("Read")]
        public async Task<IActionResult> Index()
        {
            ViewBag.ListCount = await context.UserAccess.Where(t => t.UserName != "SuperAdmin").CountAsync();
            return View();
        }
        [HttpGet]
        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> Create(Int32 Id)
        {
            var objUsr = context.UserAccess.Where(m => m.UserID == Id && m.UserName != "SuperAdmin").FirstOrDefault();

            var result = new UserAccess()
            {
                UserID = objUsr.UserID,
                UserName = objUsr.UserName,
                UserMail = objUsr.UserMail,               
                RoleName = objUsr.RoleName,
                StartDate = objUsr.StartDate,
                EndDate = objUsr.EndDate,
                CreatedBy = objUsr.CreatedBy,
                CreatedOn = objUsr.CreatedOn,
                ModifiedBy = objUsr.ModifiedBy,
                ModifiedOn = DateTime.Now
            };
            
            //ViewBag.CustomerName = context.Customer.SingleOrDefault(t => t.CustomerID == 2); 
            //ViewBag.CustomerList = context.Customer.Where(t => t.CustomerID != 0).OrderBy(t => t.CustomerName).ToListAsync(); 

            ViewBag.User = TempData["mydata"];

            var orgList = context.Customer.ToListAsync(); 
            ViewBag.OrgList = orgList.Result;

            return View(result);
        }
        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> Create(UserAccess model)
        {
            string Name = User.Identity?.Name;
            var LoginName = context.UserAccess.Where(t => t.UserMail == Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                UserAccess obj = new()
                {
                    UserID = model.UserID,
                    UserName = model.UserName,
                    UserMail = model.UserMail,  
                    RoleName = model.RoleName,
                    CustomerID = model.CustomerID,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    CreatedBy = LoginName.UserName,
                    ModifiedBy = LoginName.UserName,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
                context.UserAccess.Update(obj);

                List<User_Customer> usr = new List<User_Customer>();
                for (int i = 0; i < model.CustomerID.Count; i++)
                {
                    usr.Add(new User_Customer { UserID = obj.UserID, CustomerID = model.CustomerID[i] });
                    context.User_Customer.AddRange(usr);
                }

                context.SaveChanges();
                TempData["UserSuccess"] = "User successfully Created!";

                context.SaveChanges();
                return RedirectToAction("Index", "Users");
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
                ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0).OrderBy(t => t.CustomerName).ToListAsync();
                return View(model);
            }

        }
        [HttpGet]
        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(Int32 Id)
        {
            try
            {
                var objUsr = context.UserAccess.SingleOrDefault(m => m.UserID == Id && m.UserName != "SuperAdmin");
                List<int> cusList = context.User_Customer.Where(u => u.UserID == Id).Select(u => u.CustomerID).Distinct().ToList();

                var result = new UserAccess()
                {
                    UserID = objUsr.UserID,
                    UserName = objUsr.UserName,
                    UserMail = objUsr.UserMail,
                    CustomerID = cusList,
                    RoleName = objUsr.RoleName,
                    StartDate = objUsr.StartDate,
                    EndDate = objUsr.EndDate,
                    CreatedBy = objUsr.CreatedBy,
                    CreatedOn = objUsr.CreatedOn,
                    ModifiedBy = objUsr.ModifiedBy,
                    ModifiedOn = DateTime.Now
                };

                ViewBag.User = TempData["mydata"];

                var orgList = context.Customer.ToList();
                ViewBag.OrgList = orgList;

                return View(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
        [HttpPost]
        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(UserAccess model)
        {
            try
            {
                string Name = User.Identity?.Name;
                var LoginName = context.UserAccess.Where(t => t.UserMail == Name).FirstOrDefault();

                var Users = context.Users.SingleOrDefault(m => m.UserID == model.UserID);

                List<User_Customer> uc = new List<User_Customer>();
                
                var objUsr = new UserAccess()
                {
                    UserID = model.UserID,
                    UserName = model.UserName,
                    UserMail = model.UserMail,
                    CustomerID = model.CustomerID,
                    RoleName = model.RoleName,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    CreatedBy = model.CreatedBy,
                    CreatedOn = model.CreatedOn,
                    ModifiedBy = LoginName.UserName,
                    ModifiedOn = DateTime.Now
                };
                context.UserAccess.Update(objUsr);

                List<User_Customer> usr = new List<User_Customer>();
                for (int i = 0; i < model.CustomerID.Count; i++)
                {
                    usr.Add(new User_Customer { UserID = objUsr.UserID, CustomerID = model.CustomerID[i] });
                    context.User_Customer.AddRange(usr);
                }

                context.SaveChanges();
                TempData["UserSuccess"] = "User successfully updated!";
                return RedirectToAction("Index", "Users");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> _ListDataPartial(string sortOrder, string searchType, string searchString, int firstItem = 0)
        {
            //Sort and filter test data
            IEnumerable<UserAccess> query;
            if (sortOrder.ToLower(CultureInfo.InvariantCulture) == "descending")
            {
                query = context.UserAccess.Where(m => m.UserName != "SuperAdmin").OrderByDescending(m => m.UserName).AsEnumerable();
            }
            else
            {
                query = context.UserAccess.Where(m => m.UserName != "SuperAdmin").OrderBy(m => m.UserName);
            }
            if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchString))
            {
                if (searchType.Equals(SearchTypeConstant.CONTAIN, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    (m.UserName is not null && m.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    //|| (m.UserDesc is not null && m.UserDesc.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    );
                }
                else if (searchType.Equals(SearchTypeConstant.STARTWITH, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    (m.UserName is not null && m.UserName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase))
                        //||(m.UserDesc is not null && m.UserDesc.StartsWith(searchString, StringComparison.OrdinalIgnoreCase))
                        );
                }
                else if (searchType.Equals(SearchTypeConstant.ENDWITH, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    (m.UserName is not null && m.UserName.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)) 
                     //||(m.UserDesc is not null && m.UserDesc.EndsWith(searchString, StringComparison.OrdinalIgnoreCase))
                    );
                }
                else if (searchType.Equals(SearchTypeConstant.EQUAL, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(m =>
                    (m.UserName is not null && m.UserName.Equals(searchString, StringComparison.OrdinalIgnoreCase))
                     //||(m.UserDesc is not null && m.UserDesc.Equals(searchString, StringComparison.OrdinalIgnoreCase))
                    );
                }
            }
            // Extract a portion of data
            List<UserAccess> model = query.Skip(firstItem).Take(BATCHSIZE).ToList();
            if (model.Count == 0)
            {
                return StatusCode(204); // 204 := "No Content"
            }

            return PartialView(model);

        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || context.UserAccess == null)
            { return NotFound(); }

            var userAccess = await context.UserAccess.FirstOrDefaultAsync(m => m.UserID == id);
            if (userAccess == null)
            { return NotFound(); }

            return View(userAccess);
        }

        [RolePermissionAuthorize("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int UserID)
        {
            if (context.UserAccess == null)
            {
                return Problem("Entity set 'ApplicationDbContext.userAccess' is null.");
            }
            var userAccess = await context.UserAccess.FindAsync(UserID);
            if (userAccess != null)
            {
                context.UserAccess.Remove(userAccess);
            }
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
