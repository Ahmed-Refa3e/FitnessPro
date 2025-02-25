using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.UserDTO
{
    public class UpdateProfileDTO
    {
        [Required, MaxLength(30)]
        public required string FirstName { get; set; }
        [Required, MaxLength(30)]
        public required string LastName { get; set; }
        [Required]
        public required string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
        public string? Bio { get; set; }
    }
}
