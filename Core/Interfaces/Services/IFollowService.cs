using Core.DTOs.GeneralDTO;
using Core.Entities.Identity;

namespace Core.Interfaces.Services
{
    public interface IFollowService
    {
        Task<Generalresponse> FollowUserAsync(ApplicationUser applicationUser, string followedId);
        Task<Generalresponse> UnfollowUserAsync(ApplicationUser applicationUser, string followedId);
        Task<bool> IsFollowingUserAsync(string userId, string followedId);

        Task<Generalresponse> FollowGymAsync(ApplicationUser applicationUser, int gymId);
        Task<Generalresponse> UnfollowGymAsync(ApplicationUser applicationUser, int gymId);
        Task<bool> IsFollowingGymAsync(string userId, int gymId);

        Task<Generalresponse> FollowShopAsync(ApplicationUser applicationUser, int shopId);
        Task<Generalresponse> UnfollowShopAsync(ApplicationUser applicationUser, int shopId);
        Task<bool> IsFollowingShopAsync(string userId, int shopId);
    }
}
