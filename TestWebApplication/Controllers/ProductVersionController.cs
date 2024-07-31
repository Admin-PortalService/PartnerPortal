using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class ProductVersionController : Controller
    {
        private readonly ApplicationDbContext context;
        public ProductVersionController(ApplicationDbContext context)
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
                var productVer = context.ProductVersion.OrderByDescending(x => x.ProductVerId).FirstOrDefault();

                ViewBag.Role = Role;

                return View(productVer);
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
                var productVer = context.ProductVersion.SingleOrDefault(x => x.ProductVerId == Id);
                var result = new ProductVersion()
                {
                    ProductVerId = productVer.ProductVerId,
                    Description = productVer.Description,
                };
                return View(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Edit(ProductVersion productVer)
        {
            try
            {
                ModelState.Clear();

                ProductVersion obj = new ProductVersion();

                obj.Description = productVer.Description;

                context.ProductVersion.Add(obj);
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
