using Core.Entities.Identity;

namespace Core.Entities.OnlineTrainingEntities
{
    public class OnlineTrainingSubscription
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int OnlineTrainingId { get; set; }
        public OnlineTraining? OnlineTraining { get; set; }
        public required string TraineeID { get; set; }
        public Trainee? Trainee { get; set; }
    }
}
