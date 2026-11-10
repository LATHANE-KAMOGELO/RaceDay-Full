
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    public class OrganiserController : Controller
    {
        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            return View();
        }
    }
}
