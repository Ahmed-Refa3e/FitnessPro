using Core.Entities.GymEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Identity
{
    public class GymFollow
    {
        public required string FollowerId { get; set; }
        public ApplicationUser? FollowerUser { get; set; }
        public int GymId { get; set; }
        public Gym? Gym { get; set; }
    }
}
