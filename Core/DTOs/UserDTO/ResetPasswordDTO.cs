using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UserDTO
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string verificationCode { get; set; }
        public string newPassword { get; set; }
    }
}
