using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class RegisterCoachDTO
    {
        [Required]
        [MaxLength(30)]
        public required string FirstName { get; set; }
        [Required]

        [MaxLength(30)]
        public required string LastName { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; set; }
        [Required]
        public required string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string? Bio { get; set; }
    }
}
