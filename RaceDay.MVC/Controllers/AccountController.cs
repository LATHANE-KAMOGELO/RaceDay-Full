
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;
using RaceDay.MVC.ViewModels;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.PostAsync<RegisterViewModel, AuthResponse>(
                    "api/Auth/register", model);

                if (response is null || string.IsNullOrWhiteSpace(response.Token) ||
                    response.Role != "Participant")
                {
                    ModelState.AddModelError("", "Registration could not be completed.");
                    return View(model);
                }

                HttpContext.Session.SetString("JWToken", response.Token);
                HttpContext.Session.SetString("UserRole", response.Role);
                HttpContext.Session.SetString("UserName", response.FirstName);
                HttpContext.Session.SetInt32("UserId", response.UserId);

                return RedirectToAction("Profile", "Participants");
            }
            catch (HttpRequestException exception)
            {
                ModelState.AddModelError(
                    "",
                    exception.StatusCode == System.Net.HttpStatusCode.Conflict
                        ? "An account with this email already exists."
                        : "Unable to create your account. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.PostAsync<
                    LoginViewModel, AuthResponse>(
                    "api/Auth/login", model);

                if (response == null || string.IsNullOrEmpty(response.Token))
                {
                    ModelState.AddModelError("", "Login failed.");
                    return View(model);
                }

                HttpContext.Session.SetString("JWToken", response.Token);
                HttpContext.Session.SetString("UserRole", response.Role);
                HttpContext.Session.SetString("UserName", response.FirstName);
                HttpContext.Session.SetInt32("UserId", response.UserId);

                if (response.Role == "Organiser")
                    return RedirectToAction("Dashboard", "Organiser");

                return response.Role == "Participant"
                    ? RedirectToAction("Profile", "Participants")
                    : RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

}
