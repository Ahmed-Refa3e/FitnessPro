using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class ResetPasswordDTO
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
