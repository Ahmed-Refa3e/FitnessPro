using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(30, ErrorMessage = "First name cannot exceed 30 characters.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(30, ErrorMessage = "Last name cannot exceed 30 characters.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 30 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime DateOfBirth { get; set; }
    }
}
