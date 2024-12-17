using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.OnlineTrainingDTO
{
    public class GetOnlineTrainingGroupDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OfferPrice { get; set; }
        public byte NoOfSessionsPerWeek { get; set; }
        public byte DurationOfSession { get; set; } // Duration in minutes
        public DateTime? OfferEnded { get; set; }
        public DateTime? SubscriptionClosed { get; set; }
        public bool IsAvailable {  get; set; }
    }
}
