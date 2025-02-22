using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class VerifyCodeDTO
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string verificationCode { get; set; }
    }
}
