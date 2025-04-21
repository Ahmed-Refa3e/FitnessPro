using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.PostDTO
{
    public class AddLikeDTO
    {
        [Required]
        public int OwnerId { get; set; }
        [Required]
        public string Type { get; set; }
    }
}
