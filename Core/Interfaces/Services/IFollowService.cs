using Core.DTOs.GeneralDTO;
using Core.Entities.Identity;

namespace Core.Interfaces.Services
{
    public interface IFollowService
    {
        Task<GeneralResponse> FollowUserAsync(ApplicationUser applicationUser, string followedId);
        Task<GeneralResponse> UnfollowUserAsync(ApplicationUser applicationUser, string followedId);
        Task<bool> IsFollowingUserAsync(string userId, string followedId);

        Task<GeneralResponse> FollowGymAsync(ApplicationUser applicationUser, int gymId);
        Task<GeneralResponse> UnfollowGymAsync(ApplicationUser applicationUser, int gymId);
        Task<bool> IsFollowingGymAsync(string userId, int gymId);

        Task<GeneralResponse> FollowShopAsync(ApplicationUser applicationUser, int shopId);
        Task<GeneralResponse> UnfollowShopAsync(ApplicationUser applicationUser, int shopId);
        Task<bool> IsFollowingShopAsync(string userId, int shopId);
    }
}
