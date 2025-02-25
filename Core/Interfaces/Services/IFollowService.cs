using Core.DTOs.GeneralDTO;
using Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IFollowService
    {
        Task<Generalresponse> FollowUserAsync(ApplicationUser applicationUser, string followedId);
        Task<Generalresponse> UnfollowUserAsync(ApplicationUser applicationUser, string followedId);

        Task<Generalresponse> FollowGymAsync(ApplicationUser applicationUser, int gymId);
        Task<Generalresponse> UnfollowGymAsync(ApplicationUser applicationUser, int gymId);

        Task<Generalresponse> FollowShopAsync(ApplicationUser applicationUser, int shopId);
        Task<Generalresponse> UnfollowShopAsync(ApplicationUser applicationUser, int shopId);
    }
}
