using Core.Entities.Identity;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OnlineTrainingEntities
{
    public class OnlineTraining
    {
        public int Id { get; set; }
        public string? CoachID { get; set; }
        public Coach? Coach { get; set; }
        public ICollection<OnlineTrainingSubscription>? OnlineTrainingSubscriptions { get; set; }
        public string? Title { get; set; }
        public DurationUnit DurationUnit {  get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OfferPrice { get; set; }
        public DateTime? OfferEnded { get; set; }
        public DateTime? SubscriptionClosed { get; set; }
        public bool IsAvailable => SubscriptionClosed <= DateTime.Now;
    }
}
