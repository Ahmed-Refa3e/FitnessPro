using Core.Entities.Identity;
using Core.Entities.ShopEntities;

namespace Core.Entities.FollowEntities
{
    public class ShopFollow
    {
        public required string FollowerId { get; set; }
        public ApplicationUser? FollowerUser { get; set; }
        public int ShopId { get; set; }
        public Shop? Shop { get; set; }
    }
}
