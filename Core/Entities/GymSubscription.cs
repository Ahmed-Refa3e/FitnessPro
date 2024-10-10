using Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class GymSubscription
    {
        [Key]
        public int SubscriptionID { get; set; }
        public required string TraineeID { get; set; }
        public required Trainee Trainee { get; set; }
        public required int GymID { get; set; }
        public required Gym Gym { get; set; }
        public required string SubscriptionType { get; set; } // Monthly, Yearly, etc.
        public required int Price { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }
}
