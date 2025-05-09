using Core.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.OnlineTrainingEntities
{
    public class OnlineTrainingSubscription
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(30);
        public int OnlineTrainingId { get; set; }
        [ForeignKey("OnlineTrainingId")]
        public OnlineTraining? OnlineTraining { get; set; }
        public string? TraineeID { get; set; }
        [ForeignKey("TraineeID")]
        public Trainee? Trainee { get; set; }
        [NotMapped]
        public bool IsActive => DateTime.Now <= EndDate;
        public string? StripePaymentIntentId { get; set; } // Stripe payment intent ID for tracking payments
    }
}
