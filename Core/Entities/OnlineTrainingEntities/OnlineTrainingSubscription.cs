using Core.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.OnlineTrainingEntities
{
    public class OnlineTrainingSubscription
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int OnlineTrainingId { get; set; }
        [ForeignKey("OnlineTrainingId")]
        public OnlineTraining? OnlineTraining { get; set; }
        public string? TraineeID { get; set; }
        [ForeignKey("TraineeID")]
        public Trainee? Trainee { get; set; }
    }
}
