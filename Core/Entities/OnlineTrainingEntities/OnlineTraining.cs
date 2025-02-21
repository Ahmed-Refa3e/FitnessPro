using Core.Entities.Identity;
using Core.Enums;

namespace Core.Entities.OnlineTrainingEntities
{
    public class OnlineTraining
    {
        public int Id { get; set; }
        public required string CoachID { get; set; }
        public required Coach Coach { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TrainingType TrainingType { get; set; }
        public decimal Price { get; set; }
        public byte NoOfSessionsPerWeek { get; set; }
        public byte DurationOfSession { get; set; } // Duration in minutes   
        public ICollection<OnlineTrainingSubscription>? OnlineTrainingSubscriptions { get; set; }
    }
}
