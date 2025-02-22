using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class LoginDTO
    {
        [EmailAddress]
        public required string Email { get; set; }
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
