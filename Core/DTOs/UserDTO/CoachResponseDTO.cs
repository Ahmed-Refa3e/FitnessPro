using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UserDTO
{
    public class CoachResponseDTO
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public required string Gender { get; set; }
        public DateTime JoinedDate { get; set; }
        public double Rating { get; set; }
    }
}
