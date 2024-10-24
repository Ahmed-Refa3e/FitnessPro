using Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.GymEntities
{
    public class Gym
    {
        public int GymID { get; set; }

        [MaxLength(30)]
        public required string GymName { get; set; }
        public string? PictureUrl { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public required int MonthlyPrice { get; set; }
        public int? YearlyPrice { get; set; }
        public int? FortnightlyPrice { get; set; }
        public int? SessionPrice { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string? CoachID { get; set; }
        public Coach? Owner { get; set; }

        //navigation property
        public ICollection<GymSubscription>? GymSubscriptions { get; set; }

        public ICollection<GymRating>? Ratings { get; set; }
    }
}
