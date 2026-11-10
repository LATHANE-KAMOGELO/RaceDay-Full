
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Distance { get; set; }

        [Required]
        public int EventId { get; set; }

        public Event? Event { get; set; }

        public int MaximumParticipants { get; set; }

        public decimal EntryFee { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
