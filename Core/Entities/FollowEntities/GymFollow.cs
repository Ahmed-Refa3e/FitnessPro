using Core.Entities.GymEntities;
using Core.Entities.Identity;

namespace Core.Entities.FollowEntities
{
    public class GymFollow
    {
        public required string FollowerId { get; set; }
        public ApplicationUser? FollowerUser { get; set; }
        public int GymId { get; set; }
        public Gym? Gym { get; set; }
    }
}
