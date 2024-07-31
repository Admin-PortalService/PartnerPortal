using Microsoft.AspNetCore.Mvc;

namespace TestWebApplication.Controllers
{
    public class MaterialController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
