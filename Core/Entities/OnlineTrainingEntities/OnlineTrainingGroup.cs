using Core.DTOs.OnlineTrainingDTO;
using Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.OnlineTrainingEntities
{
    public class OnlineTrainingGroup :OnlineTraining 
    {
        
        public byte NoOfSessionsPerWeek { get; set; }
        public byte DurationOfSession { get; set; } // Duration in minutes
        public DateTime StartDate { get; set; }
        public OnlineTrainingGroup()
        {
            
        }
        public OnlineTrainingGroup(AddOnlineTrainingGroupDTO group)
        {
            this.CoachID = group.CoachID;
            this.Title = group.Title;
            this.Description = group.Description;
            this.Price = group.Price;
            this.OfferPrice = group.OfferPrice;
            this.NoOfSessionsPerWeek = group.NoOfSessionsPerWeek;
            this.DurationOfSession = group.DurationOfSession;
            this.SubscriptionClosed = group.SubscriptionClosed;
            this.OfferEnded = group.OfferEnded;
            StartDate= DateTime.Now;
        }
    }
}
