using Core.DTOs.OnlineTrainingDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OnlineTrainingEntities
{
    public class OnlineTrainingPrivate : OnlineTraining
    {
        public OnlineTrainingPrivate()
        {
            
        }
        public OnlineTrainingPrivate(AddOnlineTrainingPrivateDTO privat)
        {
            this.CoachID = privat.CoachID;
            this.Title = privat.Title;
            this.Description = privat.Description;
            this.Price = privat.Price;
            this.OfferPrice = privat.OfferPrice;
            this.SubscriptionClosed = privat.SubscriptionClosed;
            this.OfferEnded = privat.OfferEnded;
        }
    }
}
