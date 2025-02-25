using Core.Entities.Identity;

namespace Core.Entities.FollowEntities
{
    public class UserFollow
    {
        public required string FollowerId { get; set; }
        public ApplicationUser? FollowerUser { get; set; } // user who follow 

        public required string FollowingId { get; set; }
        public ApplicationUser? FollowingUser { get; set; } // The user who was followed
    }
}
