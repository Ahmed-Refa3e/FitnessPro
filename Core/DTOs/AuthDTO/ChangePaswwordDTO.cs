using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class ChangePaswwordDTO
    {
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }
        [DataType(DataType.Password)]
        public required string OldPassword { get; set; }
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }
    }
}
