using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.PostDTO
{
    public class AddCoachPostDTO : AddPostDTO
    {
        [Required]
        public string CoachId { get; set; }
    }
}
