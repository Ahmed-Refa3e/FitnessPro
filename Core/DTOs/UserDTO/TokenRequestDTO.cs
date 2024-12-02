using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UserDTO
{
    public class TokenRequestDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
