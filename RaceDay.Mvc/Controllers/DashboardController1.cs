
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Check if the user is logged in
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get the user's role from the session
            var role = HttpContext.Session.GetString("UserRole");

            ViewBag.Role = role;

            return View();
        }
    }
}
