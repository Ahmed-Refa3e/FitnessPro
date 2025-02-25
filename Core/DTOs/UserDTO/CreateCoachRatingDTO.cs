using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.UserDTO
{
    public class CreateCoachRatingDTO
    {
        [Required]
        public string coachId { get; set; }
        [Range(1, 5)]
        [Required]
        public int ratingValue { get; set; }
        public string? Review { get; set; }

    }
}
