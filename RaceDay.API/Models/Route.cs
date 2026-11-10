
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Route
    {
        [Key]
        public int RouteId { get; set; }

        [Required]
        [StringLength(150)]
        public string RouteName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public decimal Distance { get; set; }

        [StringLength(200)]
        public string? StartPoint { get; set; }

        [StringLength(200)]
        public string? FinishPoint { get; set; }

        public string? RouteMapUrl { get; set; }

        [Required]
        public int EventId { get; set; }

        public Event? Event { get; set; }
    }
}
