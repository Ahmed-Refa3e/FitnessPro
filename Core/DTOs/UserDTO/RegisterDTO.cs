using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.UserDTO
{
    public class RegisterDTO
    {
        [MaxLength(30)]
        public required string FirstName { get; set; }

        [MaxLength(30)]
        public required string LastName { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
        public required string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? Bio { get; set; }

        public string? ProfilePictureUrl { get; set; }

    }
}
