using Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.OnlineTrainingEntities
{
    public class CoachRating
    {
        public int CoachRatingId { get; set; }
        public required string CoachId { get; set; }
        public Coach? Coach { get; set; }
        public string? TraineeId { get; set; }
        public Trainee? Trainee { get; set; }

        public string? Content { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
