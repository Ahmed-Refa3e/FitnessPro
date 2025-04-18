using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.PostDTO
{
    public class AddCommentDTO
    {
        [Required]
        public int OwnerId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Content { get; set; }
    }
}
