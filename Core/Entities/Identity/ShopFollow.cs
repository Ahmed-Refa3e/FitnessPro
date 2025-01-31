using Core.Entities.GymEntities;
using Core.Entities.ShopEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Identity
{
    public class ShopFollow
    {
        public required string FollowerId { get; set; }
        public ApplicationUser? FollowerUser { get; set; }
        public int ShopId { get; set; }
        public Shop? Shop { get; set; }
    }
}
