using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UserDTO
{
    public class ConfirmEmailDTO
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}
