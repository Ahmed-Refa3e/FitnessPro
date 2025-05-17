using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.PostDTO
{
    public class AddCommentOnCommentDTO
    {
        [Required]
        public int CommentId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Content { get; set; }
    }
}
