using Core.Entities.Identity;
using Core.Enums;
using System.Text.Json.Serialization;

namespace Core.Entities.OnlineTrainingEntities
{
    public class OnlineTraining
    {
        public int Id { get; set; }
        public required string CoachID { get; set; }
        [JsonIgnore]
        public Coach? Coach { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string TrainingType { get; set; }
        public decimal Price { get; set; }
        public int NoOfSessionsPerWeek { get; set; }
        public int DurationOfSession { get; set; } // Duration in minutes   
        public ICollection<OnlineTrainingSubscription>? OnlineTrainingSubscriptions { get; set; }
    }
}
