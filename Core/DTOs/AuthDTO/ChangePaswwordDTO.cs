using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class ChangePaswwordDTO
    {
        [Required]
        [DataType(DataType.Password)]
        public required string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 30 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public required string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
