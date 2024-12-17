using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.OnlineTrainingDTO
{
    public class UpdateOnlineTrainingPrivateDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal? OfferPrice { get; set; }
        public DateTime? OfferEnded { get; set; }
        public DateTime? SubscriptionClosed { get; set; }
    }
}
