using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.UserDTO
{
    public class UpdateCoachRatingDTO
    {
        [Range(1, 5)]
        [Required]
        public int RatingValue { get; set; }
        public string? Content { get; set; }
    }
}
