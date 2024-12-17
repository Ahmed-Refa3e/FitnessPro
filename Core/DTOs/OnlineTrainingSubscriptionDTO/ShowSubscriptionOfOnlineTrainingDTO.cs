using Core.Entities.Identity;
using Core.Entities.OnlineTrainingEntities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.OnlineTrainingSubscriptionDTO
{
    public class ShowSubscriptionOfOnlineTrainingDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } 
        public string? TraineeID { get; set; }
        public string TraineeName { get; set; }
        public ShowSubscriptionOfOnlineTrainingDTO()
        {
            
        }
        public ShowSubscriptionOfOnlineTrainingDTO(OnlineTrainingSubscription subscription)
        {
            this.StartDate = subscription.StartDate;
            this.EndDate = subscription.EndDate;
            this.IsActive = subscription.EndDate >= DateTime.Now;
            this.TraineeID= subscription.TraineeID;
            this.TraineeName = subscription.Trainee.FirstName + subscription.Trainee.LastName;
        }
    }
}
