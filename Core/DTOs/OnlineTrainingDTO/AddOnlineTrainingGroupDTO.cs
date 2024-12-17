using Core.Entities.Identity;
using Core.Entities.OnlineTrainingEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.OnlineTrainingDTO
{
    public class AddOnlineTrainingGroupDTO
    {
        [Required]
        public string CoachID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal? OfferPrice { get; set; }
        [Required]
        public byte NoOfSessionsPerWeek { get; set; }
        [Required]
        public byte DurationOfSession { get; set; } // Duration in minutes
        public DateTime? OfferEnded { get; set; }
        public DateTime? SubscriptionClosed { get; set; }
    }
}
