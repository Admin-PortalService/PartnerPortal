using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class AssitToolsController : Controller
    {
        private readonly ApplicationDbContext context;
        public AssitToolsController(ApplicationDbContext context)
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

                var Role = context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).Distinct().FirstOrDefault();

                var assist = context.ImplementationAssist.OrderByDescending(x => x.AssistId).FirstOrDefault();

                ViewBag.Role = Role;

                return View(assist);
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
                var assist = context.ImplementationAssist.SingleOrDefault(x => x.AssistId == Id);
                var result = new ImplementationAssist()
                {
                    AssistId = assist.AssistId,
                    Description = assist.Description,
                };
                return View(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Edit(ImplementationAssist assist)
        {
            try
            {
                ModelState.Clear();

                ImplementationAssist obj = new ImplementationAssist();

                obj.Description = assist.Description;

                context.ImplementationAssist.Add(obj);
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
