using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.AuthDTO
{
    public class GoogleAuthDTO
    {
        [Required]
        public required string IdToken { get; set; }
        [Required]
        public required string AccessToken { get; set; }
    }
}
