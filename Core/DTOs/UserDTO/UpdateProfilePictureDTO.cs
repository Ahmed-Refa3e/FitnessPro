using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.UserDTO
{
    public class UpdateProfilePictureDTO
    {
        [Required]
        public required IFormFile ProfilePicture { get; set; }
    }
}
