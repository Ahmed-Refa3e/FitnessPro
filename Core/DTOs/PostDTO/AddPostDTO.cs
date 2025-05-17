using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.PostDTO
{
    public class AddPostDTO
    {
        [Required]
        public string Content { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
