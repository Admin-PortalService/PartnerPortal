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
    public class IssueTypeController : BaseController
    {
        private readonly ApplicationDbContext context;
        public IssueTypeController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [RolePermissionAuthorize("Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.ListCount = await context.IssueType.CountAsync();
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public IActionResult Create(IssueType model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    IssueType obj = new()
                    {
                        IssueCode = model.IssueCode,
                        IssueTypeName = model.IssueTypeName,
                        IssueTypeDesc = model.IssueTypeDesc,
                    };

                    context.IssueType.Add(obj);
                    context.SaveChanges();
                    TempData["Success"] = "Issue Type Successfully Created.";
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
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(Int32 Id)
        {
            try
            {
                var objIssue = context.IssueType.SingleOrDefault(m => m.IssueTypeID == Id);
                var result = new IssueType()
                {
                    IssueTypeID = objIssue.IssueTypeID,
                    IssueCode = objIssue.IssueCode,
                    IssueTypeName = objIssue.IssueTypeName,
                    IssueTypeDesc = objIssue.IssueTypeDesc,
                };


                return View(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpPost]
        [RolePermissionAuthorize("Update")]
        public IActionResult Edit(IssueType Issue)
        {
            try
            {
                var objIssue = new IssueType()
                {
                    IssueTypeID = Issue.IssueTypeID,
                    IssueCode = Issue.IssueCode,
                    IssueTypeName = Issue.IssueTypeName,
                    IssueTypeDesc = Issue.IssueTypeDesc
                };
                context.IssueType.Update(objIssue);
                context.SaveChanges();
                //alert msg
                TempData["Success"] = "Issue Type Successfully Modified.";
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
            //Sort and filter test data
            try
            {
                IEnumerable<IssueType> query;
                if (sortOrder.ToLower(CultureInfo.InvariantCulture) == "descending")
                {
                    query = context.IssueType.OrderByDescending(m => m.IssueTypeName).AsEnumerable();
                }
                else
                {
                    query = context.IssueType.OrderBy(m => m.IssueTypeName);
                }
                if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchString))
                {
                    if (searchType.Equals(SearchTypeConstant.CONTAIN, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.IssueTypeName is not null && m.IssueTypeName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.IssueTypeDesc is not null && m.IssueTypeDesc.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.STARTWITH, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.IssueTypeName is not null && m.IssueTypeName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                            (m.IssueTypeDesc is not null && m.IssueTypeDesc.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.ENDWITH, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.IssueTypeName is not null && m.IssueTypeName.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.IssueTypeDesc is not null && m.IssueTypeDesc.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.EQUAL, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.IssueTypeName is not null && m.IssueTypeName.Equals(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.IssueTypeDesc is not null && m.IssueTypeDesc.Equals(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                }
                // Extract a portion of data
                List<IssueType> model = query.Skip(firstItem).Take(BATCHSIZE).ToList();
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
                if (id == null || context.IssueType == null)
                { return NotFound(); }

                var module = await context.IssueType.FirstOrDefaultAsync(m => m.IssueTypeID == id);
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
        public async Task<IActionResult> DeleteConfirmed(int IssueTypeID)
        {
            try
            {

                if (context.IssueType == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.IssueType' is null.");
                }
                var issue = await context.IssueType.FirstOrDefaultAsync(m => m.IssueTypeID == IssueTypeID);
                var incident = await context.Incident.FirstOrDefaultAsync(m => m.Category == issue.IssueTypeName);
                if (incident != null)
                {
                    TempData["ErrorMsg"] = "Already used in Incident. Cannot Delete.";
                    return RedirectToAction(nameof(Index));
                    //return Problem("Already used in Incident. Cannot Delete!!");
                }
                var issueType = await context.IssueType.FindAsync(IssueTypeID);
                if (issueType != null)
                {
                    context.IssueType.Remove(issueType);
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
