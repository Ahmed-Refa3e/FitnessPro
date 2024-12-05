using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.UserDTO
{
    public class VerifyCodeDTO
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string verificationCode { get; set; }
    }
}
