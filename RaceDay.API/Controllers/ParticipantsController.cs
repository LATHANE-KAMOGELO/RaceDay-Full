using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs.Participants;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Participant")]
    public sealed class ParticipantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParticipantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<ParticipantProfileResponse>> GetMyProfile(
            CancellationToken cancellationToken)
        {
            if (!TryGetParticipantId(out var participantId))
                return Unauthorized();

            var profile = await _context.Users
                .AsNoTracking()
                .Where(user => user.UserId == participantId &&
                               user.Role == "Participant" && user.IsActive)
                .Select(user => new ParticipantProfileResponse
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                })
                .FirstOrDefaultAsync(cancellationToken);

            return profile is null ? NotFound() : Ok(profile);
        }

        [HttpPut("me")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<ParticipantProfileResponse>> UpdateMyProfile(
            UpdateParticipantProfileRequest request,
            CancellationToken cancellationToken)
        {
            if (!TryGetParticipantId(out var participantId))
                return Unauthorized();

            var participant = await _context.Users.FirstOrDefaultAsync(
                user => user.UserId == participantId &&
                        user.Role == "Participant" && user.IsActive,
                cancellationToken);

            if (participant is null)
                return NotFound();

            participant.FirstName = request.FirstName.Trim();
            participant.LastName = request.LastName.Trim();
            participant.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null
                : request.PhoneNumber.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new ParticipantProfileResponse
            {
                UserId = participant.UserId,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                Email = participant.Email,
                PhoneNumber = participant.PhoneNumber
            });
        }

        private bool TryGetParticipantId(out int participantId)
        {
            return int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out participantId);
        }
    }
}
