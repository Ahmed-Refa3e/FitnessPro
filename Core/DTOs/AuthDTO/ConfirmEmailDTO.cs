namespace Core.DTOs.AuthDTO
{
    public class ConfirmEmailDTO
    {
        public required string Email { get; set; }
        public required string VerificationCode { get; set; }
    }
}
