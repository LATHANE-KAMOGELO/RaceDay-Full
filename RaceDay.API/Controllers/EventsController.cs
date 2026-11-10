
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs.Events;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View all published events.
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .Where(e => e.IsPublished)
                .OrderBy(e => e.EventDate)
                .Select(e => new EventResponse
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Distance = e.Distance,
                    EventType = e.EventType,
                    BannerImageUrl = e.BannerImageUrl,
                    IsPublished = e.IsPublished,
                    CreatedAt = e.CreatedAt,
                    OrganiserId = e.OrganiserId
                })
                .ToListAsync();

            return Ok(events);
        }

        // View one published event.
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Where(e => e.EventId == id && e.IsPublished)
                .Select(e => new EventResponse
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Distance = e.Distance,
                    EventType = e.EventType,
                    BannerImageUrl = e.BannerImageUrl,
                    IsPublished = e.IsPublished,
                    CreatedAt = e.CreatedAt,
                    OrganiserId = e.OrganiserId
                })
                .FirstOrDefaultAsync();

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            return Ok(eventItem);
        }

        // View all events belonging to the logged-in organiser.
        [Authorize(Roles = "Organiser")]
        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
                return Unauthorized();

            var events = await _context.Events
                .Where(e => e.OrganiserId == organiserId)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new EventResponse
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Distance = e.Distance,
                    EventType = e.EventType,
                    BannerImageUrl = e.BannerImageUrl,
                    IsPublished = e.IsPublished,
                    CreatedAt = e.CreatedAt,
                    OrganiserId = e.OrganiserId
                })
                .ToListAsync();

            return Ok(events);
        }

        // View one event belonging to the logged-in organiser.
        [Authorize(Roles = "Organiser")]
        [HttpGet("manage/{id}")]
        public async Task<IActionResult> GetEventForOrganiser(int id)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
                return Unauthorized();

            var eventItem = await _context.Events
                .Where(e => e.EventId == id &&
                            e.OrganiserId == organiserId)
                .Select(e => new EventResponse
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Distance = e.Distance,
                    EventType = e.EventType,
                    BannerImageUrl = e.BannerImageUrl,
                    IsPublished = e.IsPublished,
                    CreatedAt = e.CreatedAt,
                    OrganiserId = e.OrganiserId
                })
                .FirstOrDefaultAsync();

            if (eventItem == null)
                return NotFound(new
                {
                    message = "Event not found or you do not own it."
                });

            return Ok(eventItem);
        }

        // Create an event. Organisers only.
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            CreateEventRequest request)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
                return Unauthorized();

            var newEvent = new Event
            {
                EventName = request.EventName,
                Description = request.Description ?? string.Empty,
                EventDate = request.EventDate,
                Location = request.Location,
                Distance = request.Distance,
                EventType = request.EventType,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow,
                OrganiserId = organiserId
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEventForOrganiser),
                new { id = newEvent.EventId },
                new
                {
                    message = "Event created successfully.",
                    eventId = newEvent.EventId
                });
        }

        // Update an event. Only its organiser can update it.
        [Authorize(Roles = "Organiser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(
            int id, UpdateEventRequest request)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
                return Unauthorized();

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e =>
                    e.EventId == id &&
                    e.OrganiserId == organiserId);

            if (eventItem == null)
                return NotFound(new
                {
                    message = "Event not found or you do not own it."
                });

            eventItem.EventName = request.EventName;
            eventItem.Description = request.Description ?? string.Empty;
            eventItem.EventDate = request.EventDate;
            eventItem.Location = request.Location;
            eventItem.Distance = request.Distance;
            eventItem.EventType = request.EventType;
            eventItem.IsPublished = request.IsPublished;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event updated successfully."
            });
        }

        // Delete an event. Only its organiser can delete it.
        [Authorize(Roles = "Organiser")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
                return Unauthorized();

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e =>
                    e.EventId == id &&
                    e.OrganiserId == organiserId);

            if (eventItem == null)
                return NotFound(new
                {
                    message = "Event not found or you do not own it."
                });

            var hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.EventId == id);

            if (hasEnrolments)
                return Conflict(new
                {
                    message = "This event cannot be deleted because participants are enrolled."
                });

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event deleted successfully."
            });
        }
    }
}
