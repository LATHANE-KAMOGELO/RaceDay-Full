
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentId { get; set; }

        [Required]
        public int ParticipantId { get; set; }

        public User? Participant { get; set; }

        [Required]
        public int EventId { get; set; }

        public Event? Event { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        public string? PaymentReference { get; set; }
    }
}
