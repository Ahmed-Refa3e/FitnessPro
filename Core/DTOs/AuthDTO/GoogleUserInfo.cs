namespace Core.DTOs.AuthDTO
{
    public class GoogleUserInfo
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Picture { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthdate { get; set; }
    }
}
