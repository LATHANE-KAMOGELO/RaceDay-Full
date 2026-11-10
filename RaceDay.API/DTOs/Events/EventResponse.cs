
namespace RaceDay.API.DTOs.Events
{
    public class EventResponse
    {
        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public decimal Distance { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string? BannerImageUrl { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreatedAt { get; set; }

        public int OrganiserId { get; set; }
    }
}
