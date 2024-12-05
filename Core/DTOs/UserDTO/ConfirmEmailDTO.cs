using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UserDTO
{
    public class ConfirmEmailDTO
    {
        public required string Email { get; set; }
        public required string VerificationCode { get; set; }
    }
}
