using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs.Participants
{
    public sealed class UpdateParticipantProfileRequest
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        [StringLength(30)]
        public string? PhoneNumber { get; set; }
    }
}
