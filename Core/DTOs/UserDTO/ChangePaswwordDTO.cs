using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.UserDTO
{
    public class ChangePaswwordDTO
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
