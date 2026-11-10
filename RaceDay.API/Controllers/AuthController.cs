
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.DTOs;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);

            if (response == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            return Ok(response);
        }
    }
}
