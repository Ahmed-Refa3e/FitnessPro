using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.UserDTO
{
    public class RegisterDTO
    {
        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public required string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? Bio { get; set; }

        public string? ProfilePictureUrl { get; set; }

    }
}
