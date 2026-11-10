using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.ViewModels
{
    public sealed class ParticipantProfileViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(30)]
        public string? PhoneNumber { get; set; }
    }
}
