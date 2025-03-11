using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UserDTO
{
    public class CoachProfileDTO : GetProfileDTO
    {
        public string? Bio { get; set; }
    }
}
