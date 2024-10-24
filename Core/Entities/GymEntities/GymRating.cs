using Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.GymEntities
{
    public class GymRating
    {
        public int GymRatingID { get; set; }

        // Rating value between 1 and 5
        [Range(1, 5)]
        public int RatingValue { get; set; }

        public string? Review { get; set; } // Optional review text

        public required string TraineeID { get; set; }
        public required Trainee Trainee { get; set; }

        public required int GymID { get; set; }
        public required Gym Gym { get; set; }

        public DateTime RatingDate { get; set; } = DateTime.Now;
    }
}