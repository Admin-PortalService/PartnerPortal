using Microsoft.AspNetCore.Mvc;

namespace TestWebApplication.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
