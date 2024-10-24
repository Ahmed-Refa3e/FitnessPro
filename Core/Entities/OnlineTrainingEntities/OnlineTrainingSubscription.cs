using Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.OnlineTrainingEntities
{
    public class OnlineTrainingSubscription
    {
        [Key]
        public int SubscriptionID { get; set; }
        public int TrainingID { get; set; }
        public required OnlineTraining Training { get; set; }
        public required string TraineeID { get; set; }
        public required Trainee Trainee { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string Status { get; set; } // Active, Completed, Cancelled
    }
}
