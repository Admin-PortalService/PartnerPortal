using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class QuotationController : BaseController
    {
        private readonly ApplicationDbContext context;
        public QuotationController(ApplicationDbContext context)
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

                var userID = context.UserAccess.Where(t => t.UserMail == userName).Select(t => t.UserID).FirstOrDefault();
                var customerIDs = context.User_Customer.Where(u => u.UserID == userID).ToList();

                List<string> userRoleIds = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().ToList();

                if (userRoleIds.Contains("363432dc-f55d-43bd-9f8c-8f764817319e"))
                {
                    ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0).OrderBy(t => t.CustomerName).ToListAsync();
                    ViewBag.ProjectList = await context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToListAsync();
                }
                else
                {
                    var customerList = new List<Customer>();
                    var projectList = new List<Projects>();

                    foreach (var cusId in customerIDs)
                    {
                        var custIDs = await context.Customer.Where(t => t.CustomerID == cusId.CustomerID).ToListAsync();
                        customerList.AddRange(custIDs);
                        var projectIDs = await context.Project.Where(t => t.CustomerID == cusId.CustomerID).ToListAsync();
                        projectList.AddRange(projectIDs);
                    }

                    ViewBag.CustomerList = customerList;
                    ViewBag.ProjectList = projectList;
                }

                ViewBag.quotation = await context.Quotation.OrderByDescending(t => t.Date).FirstOrDefaultAsync();
                ViewBag.ItemList = await context.Item.OrderBy(m => m.ItemName).ToListAsync();

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
        public async Task<IActionResult> ItemAdd()
        {
            try
            {
                ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0).OrderBy(t => t.CustomerName).ToListAsync();
                ViewBag.ProjectList = await context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToListAsync();
                ViewBag.ItemList = await context.Item.OrderBy(m => m.ItemName).ToListAsync();

                Quotation quotation = new Quotation();
                quotation.ItemQuotation.Add(new ItemQuotation() { itemId = 1 });

                return View(quotation);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> ItemAdd(Quotation quotation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (ItemQuotation item in quotation.ItemQuotation.ToList())
                    {
                        if (item.itemId == null)
                            quotation.ItemQuotation.Remove(item);
                    }

                    var desc = context.Quotation.Where(t => t.Desc == quotation.Desc).FirstOrDefault();
                    if (quotation.Desc == desc?.Desc)
                    {
                        ViewBag.error = "Description already exist.";
                        ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0).OrderBy(t => t.CustomerName).ToListAsync();
                        ViewBag.ProjectList = await context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToListAsync();
                        ViewBag.ItemList = await context.Item.OrderBy(m => m.ItemName).ToListAsync();
                        return View(quotation);
                    }

                    for (int i = 0; i < quotation.ItemQuotation.Count; i++)
                    {
                        var item = await context.Item.Where(t => t.ItemId == quotation.ItemQuotation[i].item.ItemId).AsNoTracking().FirstOrDefaultAsync();
                        quotation.ItemQuotation[i].item.ItemId = item.ItemId;
                        quotation.ItemQuotation[i].item.ItemName = item.ItemName;
                        quotation.ItemQuotation[i].item.ItemDes = item.ItemDes;
                        quotation.ItemQuotation[i].item.Remark = item.Remark;
                    }

                    context.Update(quotation);
                    context.SaveChanges();
                    TempData["Success"] = "New Quotation Successfully Created!";
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
                            errors[keyValue.Key] = keyValue.Value.Errors.Select(e => e.ErrorMessage);
                            for (int i = 0; i < keyValue.Value.Errors.Count; i++)
                            {
                                var msg1 = keyValue.Value.Errors[i].ErrorMessage;
                                string msg = msg1 + "!";
                                message.Add(msg);
                            }

                        }
                    }
                    var strings = (from o in message
                                   select o.ToString()).ToList();
                    ViewBag.msgList = strings;
                    ModelState.Clear();
                    ViewBag.CustomerList = await context.Customer.Where(t => t.CustomerID != 0).OrderBy(t => t.CustomerName).ToListAsync();
                    ViewBag.ProjectList = await context.Project.Where(p => p.ProjectID != 0).OrderBy(p => p.ProjectName).ToListAsync();
                    ViewBag.ItemList = await context.Item.OrderBy(m => m.ItemName).ToListAsync();
                    return View(quotation);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult GridFilter(int organization, int project)
        {
            try
            {
                decimal total = 0;
                List<ItemQuotation> itemQuotation = context.ItemQuotation.Include(z => z.item).Include(x => x.quotaion).Where(y => y.quotaion.Organization == organization || y.quotaion.Project == project).ToList();

                foreach (var item in itemQuotation)
                {
                    total += item.Amount;
                }

                ViewBag.TotalAmount = total;

                return PartialView("_GridFilter", itemQuotation);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public JsonResult GetItemDescAndRemark(int ItemId)
        {
            var selectedItem = context.Item.Where(x => x.ItemId == ItemId).FirstOrDefault();

            return Json(selectedItem);
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
    }
}
