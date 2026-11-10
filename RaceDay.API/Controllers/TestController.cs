
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // Public endpoint.
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok(new
            {
                message = "RaceDay API is running successfully."
            });
        }

        // Requires a valid JWT token.
        [Authorize]
        [HttpGet("secure")]
        public IActionResult Secure()
        {
            return Ok(new
            {
                message = "You have successfully accessed a protected endpoint!",
                user = User.Identity?.Name,
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }

        // Requires an Organiser role.
        [Authorize(Roles = "Organiser")]
        [HttpGet("organiser")]
        public IActionResult OrganiserOnly()
        {
            return Ok(new
            {
                message = "Welcome to the organiser area!",
                user = User.Identity?.Name,
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }
    }
}
