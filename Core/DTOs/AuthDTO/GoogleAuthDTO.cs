using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class GoogleAuthDTO
    {
        [Required]
        public required string IdToken { get; set; }
        [Required]
        public required string AccessToken { get; set; }
        [Required]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Role must be either 'Coach' or 'Trainee'")]
        public required string Role { get; set; }
    }
}
