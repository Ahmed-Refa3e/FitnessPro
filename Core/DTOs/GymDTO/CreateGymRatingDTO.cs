using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.GymDTO
{
    public class CreateGymRatingDTO
    {
        [Required]
        public required int GymID { get; set; }

        [Range(1, 5)]
        [Required]
        public required int RatingValue { get; set; }
        public string? Review { get; set; }

    }
}
