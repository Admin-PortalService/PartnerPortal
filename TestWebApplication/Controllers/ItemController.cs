using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TestWebApplication.Common;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class ItemController : BaseController
    {
        private readonly ApplicationDbContext context;
        public ItemController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [RolePermissionAuthorize("Read")]
        public async Task<IActionResult> Index()
        {
            try
            {

                ViewBag.ListCount = await context.Item.CountAsync();
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
            return View();
        }

        [HttpPost]
        [RolePermissionAuthorize("Create")]
        public async Task<IActionResult> Create(Item model)
        {
            try
            {
                await context.AddRangeAsync();
                if (ModelState.IsValid)
                {
                    Item obj = new()
                    {
                        ItemName = model.ItemName,
                        ItemDes = model.ItemDes,
                        Remark = model.Remark,
                    };

                    context.Item.Add(obj);
                    context.SaveChanges();

                    TempData["Success"] = "Item Successfully Created.";

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

        public IActionResult Edit(int Id)
        {
            try
            {
                var objItem = context.Item.SingleOrDefault(m => m.ItemId == Id);
                var result = new Item()
                {
                    ItemId = objItem.ItemId,
                    ItemName = objItem.ItemName,
                    ItemDes = objItem.ItemDes,
                    Remark = objItem.Remark
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
        public IActionResult Edit(Item item)
        {
            try
            {
                var objItem = new Item()
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    ItemDes = item.ItemDes,
                    Remark = item.Remark
                };
                context.Item.Update(objItem);
                context.SaveChanges();

                TempData["Success"] = "Item Successfully Modified";

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
                IEnumerable<Item> query;
                if (sortOrder.ToLower(CultureInfo.InvariantCulture) == "descending")
                {
                    query = context.Item.OrderByDescending(m => m.ItemName).AsEnumerable();
                }
                else
                {
                    query = context.Item.OrderBy(m => m.ItemName);
                }
                if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchString))
                {
                    if (searchType.Equals(SearchTypeConstant.CONTAIN, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.ItemName is not null && m.ItemName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.ItemDes is not null && m.ItemDes.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.STARTWITH, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.ItemName is not null && m.ItemName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                            (m.ItemDes is not null && m.ItemDes.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.ENDWITH, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.ItemName is not null && m.ItemName.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.ItemDes is not null && m.ItemDes.EndsWith(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                    else if (searchType.Equals(SearchTypeConstant.EQUAL, StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(m =>
                        (m.ItemName is not null && m.ItemName.Equals(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (m.ItemDes is not null && m.ItemDes.Equals(searchString, StringComparison.OrdinalIgnoreCase)));
                    }
                }
                // Extract a portion of data
                List<Item> model = query.Skip(firstItem).Take(BATCHSIZE).ToList();
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

        [HttpPost, ActionName("Delete")]
        [RolePermissionAuthorize("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ItemId)
        {
            try
            {

                if (context.Item == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Item' is null.");
                }
                var item = await context.Item.FindAsync(ItemId);
                if (item != null)
                {
                    context.Item.Remove(item);
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
                if (id == null || context.Item == null)
                { return NotFound(); }
                var item = await context.Item.FirstOrDefaultAsync(m => m.ItemId == id);
                if (item == null)
                { return NotFound(); }
                return View(item);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
