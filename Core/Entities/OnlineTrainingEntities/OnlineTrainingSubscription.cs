using Core.DTOs.OnlineTrainingSubscriptionDTO;
using Core.Entities.Identity;

namespace Core.Entities.OnlineTrainingEntities
{
    public class OnlineTrainingSubscription
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        // public decimal Cost { get; set; }
        public int? OnlineTrainingId { get; set; }
        public OnlineTraining? OnlineTraining { get; set; }
        public string? TraineeID { get; set; }
        public Trainee? Trainee { get; set; }
        public OnlineTrainingSubscription()
        {

        }
        public OnlineTrainingSubscription(AddSubscriptionDTO subscription, OnlineTraining training)
        {
            this.OnlineTrainingId = subscription.OnlineTrainingId;
            this.TraineeID = subscription.TraineeID;
            this.StartDate = DateTime.Now;
            //this.EndDate = (training.DurationUnit == DurationUnit.Week) ? StartDate.AddDays(7) :
            //    (training.DurationUnit == DurationUnit.Month) ? StartDate.AddMonths(1) :
            //    (training.DurationUnit == DurationUnit.QuarterYear) ? StartDate.AddMonths(3) :
            //    (training.DurationUnit == DurationUnit.HalfYear) ? StartDate.AddMonths(6) :
                StartDate.AddYears(1);
            //if (training.OfferEnded is not null && training.OfferEnded >= DateTime.Now)
            //{
            //    //this.Cost = training.OfferPrice ?? training.Price;
            //}
            //else
            //{
            //    //this.Cost = training.Price;
            //}
        }
    }
}
