using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestWebApplication.Data;
using TestWebApplication.Helpers;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Authorize]
    public class CampusController : Controller
    {
        private readonly ApplicationDbContext context;
        public CampusController(ApplicationDbContext context)
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
                var campus = context.Campus.OrderByDescending(x => x.campusId).FirstOrDefault();

                ViewBag.Role = Role;

                return View(campus);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public IActionResult Edit(int Id)
        {
            try
            {
                var campus = context.Campus.SingleOrDefault(x => x.campusId == Id);
                var result = new Campus()
                {
                    campusId = campus.campusId,
                    Description = campus.Description,
                };
                return View(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public IActionResult Edit(Campus campus)
        {
            try
            {
                ModelState.Clear();

                Campus obj = new Campus();

                obj.Description = campus.Description;

                context.Campus.Add(obj);
                context.SaveChanges();
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
