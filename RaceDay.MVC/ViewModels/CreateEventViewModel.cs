
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.ViewModels
{
    public class CreateEventViewModel
    {
        [Required]
        public string EventName { get; set; } = "";

        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; } = "";

        [Range(0.1, 1000)]
        public decimal Distance { get; set; }

        [Required]
        public string EventType { get; set; } = "";
    }
}
