
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        [Required]
        public int EnrolmentId { get; set; }

        public Enrolment? Enrolment { get; set; }

        [Required]
        public TimeSpan FinishTime { get; set; }

        [Required]
        public int FinishingPosition { get; set; }

        [Required]
        public int TotalParticipants { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; } = false;
    }
}
