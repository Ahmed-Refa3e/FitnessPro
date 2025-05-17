using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.PostDTO
{
    public class AddCommentOnPostDTO
    {
        [Required]
        public int PostId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Content { get; set; }
    }
}
