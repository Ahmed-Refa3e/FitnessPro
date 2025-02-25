namespace Core.DTOs.AuthDTO
{
    public class TokenRequestDTO
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }
}
