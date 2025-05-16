using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.PostDTO
{
    public class AddLikeOnPostDTO
    {
        [Required]
        public int PostId { get; set; }
        [Required]
        public string Type { get; set; }
    }
}
