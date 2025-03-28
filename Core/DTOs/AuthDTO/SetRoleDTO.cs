using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class SetRoleDTO
    {
        [Required]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Role must be either 'Coach' or 'Trainee'")]
        public required string Role { get; set; }
    }
}
