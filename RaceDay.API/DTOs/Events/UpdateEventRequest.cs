
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs.Events
{
    public class UpdateEventRequest
    {
        [Required]
        [StringLength(150)]
        public string EventName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(0.1, 1000)]
        public decimal Distance { get; set; }

        [Required]
        [StringLength(50)]
        public string EventType { get; set; } = string.Empty;

        public bool IsPublished { get; set; }
    }
}
