using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class PriceController : Controller
    {
        private readonly ApplicationDbContext context;
        public PriceController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [RolePermissionAuthorize("Read")]
        public IActionResult Index()
        {
            try
            {
                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userName = User.Identity?.Name;

                var Role = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().FirstOrDefault();
                var price = context.Price.OrderByDescending(x => x.PriceId).FirstOrDefault();

                ViewBag.Role = Role;

                return View(price);
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
                var price = context.Price.SingleOrDefault(x => x.PriceId == Id);
                var result = new Price()
                {
                    PriceId = price.PriceId,
                    Description = price.Description,
                };
                return View(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Edit(Price price)
        {
            try
            {
                ModelState.Clear();

                Price obj = new Price();

                obj.Description = price.Description;

                context.Price.Add(obj);
                context.SaveChanges();
                return View();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
