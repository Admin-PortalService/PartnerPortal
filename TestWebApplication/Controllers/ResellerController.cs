using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class ResellerController : Controller
    {
        private readonly ApplicationDbContext context;
        public ResellerController(ApplicationDbContext context)
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
                var reseller = context.Reseller.OrderByDescending(x => x.ResellerId).FirstOrDefault();

                ViewBag.Role = Role;

                return View(reseller);
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
                var reseller = context.Reseller.SingleOrDefault(x => x.ResellerId == Id);
                var result = new Reseller()
                {
                    ResellerId = reseller.ResellerId,
                    Description = reseller.Description,
                };
                return View(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Edit(Reseller reseller)
        {
            try
            {
                ModelState.Clear();

                Reseller obj = new Reseller();

                obj.Description = reseller.Description;

                context.Reseller.Add(obj);
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
