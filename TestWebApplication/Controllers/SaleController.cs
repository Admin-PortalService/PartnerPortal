using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext context;
        public SaleController(ApplicationDbContext context)
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
                var sale = context.Sale.OrderByDescending(x => x.SaleId).FirstOrDefault();

                ViewBag.Role = Role;

                return View(sale);
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
                var sale = context.Sale.SingleOrDefault(x => x.SaleId == Id);
                var result = new Sale()
                {
                    SaleId = sale.SaleId,
                    Description = sale.Description,
                };
                return View(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Edit(Sale sale)
        {
            try
            {
                ModelState.Clear();

                Sale obj = new Sale();

                obj.Description = sale.Description;

                context.Sale.Add(obj);
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
