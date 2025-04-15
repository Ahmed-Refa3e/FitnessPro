using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.PostDTO
{
    public class AddGymPostDTO : AddPostDTO
    {
        [Required]
        public int GymId { get; set; }
    }
}
