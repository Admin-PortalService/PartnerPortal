using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Globalization;
using TestWebApplication.Data;
using TestWebApplication.Models.ViewModels;
using TestWebApplication.Common;
using Microsoft.EntityFrameworkCore;
using System;
using TestWebApplication.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class CustomerController : BaseController
    {
        private readonly ApplicationDbContext context;
        public CustomerController(ApplicationDbContext context)
            {
                this.context = context;
            }
        [RolePermissionAuthorize("Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.ListCount = await context.Customer.CountAsync();
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
        public async Task<IActionResult>Create()
        {
            return View();
        }
        
        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> Create(Customer model)
        {
            try
            {
                string userName = User.Identity?.Name;
                var LoginName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                await context.AddRangeAsync();
                if (ModelState.IsValid)
                {
                    Customer obj = new()
                    {
                        CustomerCode = model.CustomerCode,
                        CustomerName = model.CustomerName,
                        IsActive = model.IsActive,
                        CustomerEmail = model.CustomerEmail,
                        CustomerAddress1 = model.CustomerAddress1,
                        CustomerAddress2 = model.CustomerAddress2,
                        Country = model.Country,
                        ContactPerson = model.ContactPerson,
                        ContactInfo = model.ContactInfo,
                        ContactEmail = model.ContactEmail,
                        CreatedBy = LoginName.UserName,
                        ModifiedBy = LoginName.UserName,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    };

                    context.Customer.Add(obj);
                    context.SaveChanges();
                    TempData["Success"] = "Organization Successfully Created";

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
        public IActionResult Edit(int Id)
            {
            try
            {
                ViewBag.projectList = context.Project.Where(m => m.CustomerID == Id).ToList();
                var objCust = context.Customer.SingleOrDefault(m => m.CustomerID == Id);
                var result = new Customer()
                {
                    CustomerCode = objCust.CustomerCode,
                    CustomerID = objCust.CustomerID,
                    CustomerName = objCust.CustomerName,
                    IsActive = objCust.IsActive,
                    CustomerEmail = objCust.CustomerEmail,
                    CustomerAddress1 = objCust.CustomerAddress1,
                    CustomerAddress2 = objCust.CustomerAddress2,
                    Country = objCust.Country,
                    ContactPerson = objCust.ContactPerson,
                    ContactInfo = objCust.ContactInfo,
                    ContactEmail = objCust.ContactEmail,
                    CreatedBy = objCust.CreatedBy,
                    CreatedOn = objCust.CreatedOn,
                    ModifiedBy = objCust.ModifiedBy,
                    ModifiedOn = DateTime.Now
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
        public IActionResult Edit(Customer Cust)
            {
            try
            {
                string userName = User.Identity?.Name;
                var LoginName = context.UserAccess.Where(t => t.UserMail == userName).FirstOrDefault();

                var objCust = new Customer()
                {
                    CustomerCode = Cust.CustomerCode,
                    CustomerID = Cust.CustomerID,
                    CustomerName = Cust.CustomerName,
                    IsActive = Cust.IsActive,
                    CustomerEmail = Cust.CustomerEmail,
                    CustomerAddress1 = Cust.CustomerAddress1,
                    CustomerAddress2 = Cust.CustomerAddress2,
                    Country = Cust.Country,
                    ContactPerson = Cust.ContactPerson,
                    ContactInfo = Cust.ContactInfo,
                    ContactEmail = Cust.ContactEmail,
                    CreatedBy = Cust.CreatedBy,
                    CreatedOn = Cust.CreatedOn,
                    ModifiedBy = LoginName.UserName,
                    ModifiedOn = DateTime.Now
                };

                context.Customer.Update(objCust);
                context.SaveChanges();
                //alert msg
                TempData["Success"] = "Organization Successfully Modified.";

                return RedirectToAction("Edit");
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
                IEnumerable<Customer> query;
                if (sortOrder.ToLower(CultureInfo.InvariantCulture) == "descending")
                {
                    query = context.Customer.OrderByDescending(m => m.CustomerName).AsEnumerable();
                }
                else
                {
                    query = context.Customer.OrderBy(m => m.CustomerName);
                }
                if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchString))
                {
                    if (searchType.Equals(SearchTypeConstant.CONTAIN, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.CustomerName is not null && m.CustomerName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.CustomerAddress1 is not null && m.CustomerAddress1.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.STARTWITH, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.CustomerName is not null && m.CustomerName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                            (m.CustomerAddress1 is not null && m.CustomerAddress1.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.ENDWITH, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.CustomerName is not null && m.CustomerName.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.CustomerAddress1 is not null && m.CustomerAddress1.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.EQUAL, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.CustomerName is not null && m.CustomerName.Equals(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.CustomerAddress1 is not null && m.CustomerAddress1.Equals(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                }
                // Extract a portion of data
                List<Customer> model = query.Skip(firstItem).Take(BATCHSIZE).ToList();
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
                if (id == null || context.Customer == null)
                { return NotFound(); }

                var cust = await context.Customer.FirstOrDefaultAsync(m => m.CustomerID == id);
                if (cust == null)
                { return NotFound(); }

                return View(cust);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost, ActionName("Delete")]
        [RolePermissionAuthorize("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int CustomerID)
        {
            try
            {
                if (context.Customer == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Customer' is null.");
                }
                var org = await context.Customer.FirstOrDefaultAsync(m => m.CustomerID == CustomerID);
                var project = await context.Project.FirstOrDefaultAsync(m => m.CustomerID == org.CustomerID);
                if (project != null)
                {
                    TempData["ErrorMsg"] = "Already used. Cannot Delete.";
                    return RedirectToAction(nameof(Index));
                }
                var customer = await context.Customer.FindAsync(CustomerID);
                if (customer != null)
                {
                    context.Customer.Remove(customer);
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
                if (id == null || context.Customer == null)
                { return NotFound(); }
                var cust = await context.Customer.FirstOrDefaultAsync(m => m.CustomerID == id);
                if (cust == null)
                { return NotFound(); }
                return View(cust);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}

