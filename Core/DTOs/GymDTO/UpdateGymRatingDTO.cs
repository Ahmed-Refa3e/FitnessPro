using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.GymDTO
{
    public class UpdateGymRatingDTO
    {

        [Range(1, 5)]
        [Required]
        public required int RatingValue { get; set; }
        public string? Review { get; set; }

    }
}
