using Core.Entities.Identity;
using Core.Entities.OnlineTrainingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.OnlineTrainingSubscriptionDTO
{
    public class ShowSubscriptionDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status {  get; set; }
        public int? OnlineTrainingId { get; set; }
        public string? OnlineTrainingTitle {  get; set; }
        public string? TraineeID { get; set; }
        public string? TraineeName { get; set; }
        public decimal Cost { get; set; }
        public ShowSubscriptionDTO()
        {
            
        }
        public ShowSubscriptionDTO(OnlineTrainingSubscription subscription)
        {
            this.StartDate = subscription.StartDate;
            this.EndDate = subscription.EndDate;
            this.Status = subscription.EndDate >= DateTime.Now ?"Active":"Completed";
            this.OnlineTrainingId = subscription.OnlineTrainingId;
            this.OnlineTrainingTitle = subscription.OnlineTraining.Title;
            this.TraineeID = subscription.TraineeID;
            this.TraineeName = subscription.Trainee.FirstName + subscription.Trainee.LastName;
            this.Cost = subscription.Cost;
        }
    }
}
