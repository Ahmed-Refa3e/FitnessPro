using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.AuthDTO
{
    public class TokenRequestDTO
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }
}
