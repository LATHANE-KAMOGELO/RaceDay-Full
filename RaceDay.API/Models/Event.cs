
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(150)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public decimal Distance { get; set; }

        [Required]
        [StringLength(50)]
        public string EventType { get; set; } = string.Empty;

        public string? BannerImageUrl { get; set; }

        public bool IsPublished { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Organiser who created the event
        [Required]
        public int OrganiserId { get; set; }

        public User? Organiser { get; set; }

        // Event categories
        public ICollection<Category> Categories { get; set; }
            = new List<Category>();

        // Event routes
        public ICollection<Route> Routes { get; set; }
            = new List<Route>();
    }
}
