using Core.Entities.GymEntities;
using Core.Entities.OnlineTrainingEntities;

namespace Core.Entities.Identity
{
    public class Trainee : ApplicationUser
    {
        public ICollection<GymSubscription>? GymSubscriptions { get; set; }
        public ICollection<OnlineTrainingSubscription>? OnlineTrainingSubscriptions { get; set; }
    }
}
