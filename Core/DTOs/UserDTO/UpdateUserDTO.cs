using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UserDTO
{
    public class UpdateProfileDTO
    {
        [MaxLength(30)]
        public string? FirstName { get; set; }
        [MaxLength(30)]
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }
    }
}
