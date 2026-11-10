namespace RaceDay.API.DTOs.Participants
{
    public sealed class ParticipantProfileResponse
    {
        public int UserId { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
    }
}
