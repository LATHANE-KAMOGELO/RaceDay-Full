using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;
using RaceDay.MVC.ViewModels;

namespace RaceDay.MVC.Controllers
{
    public sealed class ParticipantsController : Controller
    {
        private readonly ApiService _apiService;

        public ParticipantsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Profile(CancellationToken cancellationToken)
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant")
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account");

            try
            {
                var profile = await _apiService.GetAsync<ParticipantProfileViewModel>(
                    "api/Participants/me", token, cancellationToken);

                if (profile is null)
                {
                    ViewBag.Error = "Your profile could not be found.";
                    return View(new ParticipantProfileViewModel());
                }

                return View(profile);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to load your profile. Please try again.";
                return View(new ParticipantProfileViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Profile(
            ParticipantProfileViewModel model,
            CancellationToken cancellationToken)
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant")
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var request = new
                {
                    model.FirstName,
                    model.LastName,
                    model.PhoneNumber
                };

                var updatedProfile = await _apiService.PutAsync<object, ParticipantProfileViewModel>(
                    "api/Participants/me", request, token, cancellationToken);

                if (updatedProfile is null)
                {
                    ModelState.AddModelError("", "The profile could not be updated.");
                    return View(model);
                }

                HttpContext.Session.SetString("UserName", updatedProfile.FirstName);
                TempData["Success"] = "Your profile has been updated.";
                return RedirectToAction(nameof(Profile));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Unable to save your profile. Please try again.");
                return View(model);
            }
        }
    }
}
