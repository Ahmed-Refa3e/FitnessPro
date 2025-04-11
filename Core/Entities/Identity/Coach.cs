using Core.Entities.GymEntities;
using Core.Entities.OnlineTrainingEntities;
using Core.Entities.PostEntities;
using Core.Entities.ShopEntities;

namespace Core.Entities.Identity
{
    public class Coach : ApplicationUser
    {
        public string? Bio { get; set; }
        public Gym? Gym { get; set; }
        public ICollection<OnlineTraining>? OnlineTrainings { get; set; }
        public List<CoachPost>? Posts { get; set; }
        public List<Shop>? Shops { get; set; }
        public ICollection<CoachRating>? Ratings { get; set; }
    }
}
