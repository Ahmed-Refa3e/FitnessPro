using Core.DTOs.GeneralDTO;
using Core.Entities.FollowEntities;
using Core.Entities.Identity;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class FollowService(IUserRepository userRepository, IGymRepository gymRepository, FitnessContext context) : IFollowService
    {
        public async Task<Generalresponse> FollowGymAsync(ApplicationUser applicationUser, int gymId)
        {
            Generalresponse response = new Generalresponse();
            var checkfollow = await userRepository.GetGymFollow(applicationUser.Id, gymId);
            if (checkfollow != null)
            {
                response.IsSuccess = false;
                response.Data = "You already follow this gym";
                return response;
            }

            var user = await userRepository.GetAsync(e => e.Id == applicationUser.Id, "FollowedGyms");
            var gym = await gymRepository.GetByIdAsync(gymId);
            if (gym == null)
            {
                response.IsSuccess = false;
                response.Data = "Gym not found";
                return response;
            }

            var follow = new GymFollow
            {
                GymId = gymId,
                FollowerId = applicationUser.Id,
                FollowerUser = user,
                Gym = gym,
            };

            await userRepository.AddGymFollower(follow);
            gym.Followers?.Add(follow);
            user?.FollowedGyms?.Add(follow);
            await userRepository.SaveAsync();

            response.IsSuccess = true;
            response.Data = $"You Now Follow {gym.GymName}";
            return response;
        }

        public async Task<Generalresponse> FollowShopAsync(ApplicationUser applicationUser, int shopId)
        {
            Generalresponse response = new Generalresponse();
            var checkfollow = await userRepository.GetShopFollow(applicationUser.Id, shopId);
            if (checkfollow != null)
            {
                response.IsSuccess = false;
                response.Data = "You already follow this Shop";
                return response;
            }

            var user = await userRepository.GetAsync(e => e.Id == applicationUser.Id, "FollowedShops");
            var shop = context.Shops.Include(e => e.Followers).FirstOrDefault(e => e.Id == shopId);
            if (shop == null)
            {
                response.IsSuccess = false;
                response.Data = "Shop not found";
                return response;
            }

            var follow = new ShopFollow
            {
                ShopId = shopId,
                FollowerId = applicationUser.Id,
                FollowerUser = user,
                Shop = shop,
            };

            await userRepository.AddShopFollower(follow);
            shop.Followers?.Add(follow);
            user?.FollowedShops?.Add(follow);
            await userRepository.SaveAsync();

            response.IsSuccess = true;
            response.Data = $"You Now Follow {shop.Name}";
            return response;
        }

        public async Task<Generalresponse> FollowUserAsync(ApplicationUser applicationUser, string followedId)
        {
            Generalresponse response = new Generalresponse();
            if (applicationUser.Id == followedId)
            {
                response.IsSuccess = false;
                response.Data = "You Can't Follow Yourself";
                return response;
            }

            var user = await userRepository.GetAsync(e => e.Id == applicationUser.Id, "Following");
            var followedUser = await userRepository.GetAsync(e => e.Id == followedId, "Followers");

            if (followedUser == null)
            {
                response.IsSuccess = false;
                response.Data = "User not found";
                return response;
            }

            var checkFollow = await userRepository.GetFollow(userId: applicationUser.Id, FollowedId: followedId);
            if (checkFollow != null)
            {
                response.IsSuccess = false;
                response.Data = $"You Follow {followedUser.FirstName} {followedUser.LastName} already";
                return response;
            }

            var follow = new UserFollow
            {
                FollowerId = applicationUser.Id,
                FollowingId = followedId,
                FollowerUser = user,
                FollowingUser = followedUser
            };

            await userRepository.AddFollower(follow);
            user?.Following?.Add(follow);
            followedUser.Followers?.Add(follow);

            await userRepository.SaveAsync();
            response.IsSuccess = true;
            response.Data = $"You Now Follow {followedUser.FirstName} {followedUser.LastName}";
            return response;
        }

        public async Task<Generalresponse> UnfollowGymAsync(ApplicationUser applicationUser, int gymId)
        {
            Generalresponse response = new Generalresponse();
            var follow = await userRepository.GetGymFollow(applicationUser.Id, gymId);
            if (follow == null)
            {
                response.IsSuccess = false;
                response.Data = "You Didn't Follow This Gym";
                return response;
            }

            var user = await userRepository.GetAsync(e => e.Id == applicationUser.Id, "FollowedGyms");
            var gym = await gymRepository.GetByIdAsync(gymId);
            if (gym == null)
            {
                response.IsSuccess = false;
                response.Data = "Gym not found";
                return response;
            }

            userRepository.RemoveGymFollow(follow);
            user?.FollowedGyms?.Remove(follow);
            gym.Followers?.Remove(follow);
            await userRepository.SaveAsync();

            response.IsSuccess = true;
            response.Data = $"You unfollow {gym.GymName}";
            return response;
        }

        public async Task<Generalresponse> UnfollowShopAsync(ApplicationUser applicationUser, int shopId)
        {
            Generalresponse response = new Generalresponse();
            var follow = await userRepository.GetShopFollow(applicationUser.Id, shopId);
            if (follow == null)
            {
                response.IsSuccess = false;
                response.Data = "You Didn't Follow This Shop";
                return response;
            }

            var user = await userRepository.GetAsync(e => e.Id == applicationUser.Id, "FollowedShops");
            var shop = context.Shops.Include(e => e.Followers).FirstOrDefault(e => e.Id == shopId);
            if (shop == null)
            {
                response.IsSuccess = false;
                response.Data = "Shop not found";
                return response;
            }

            userRepository.RemoveShopFollow(follow);
            user?.FollowedShops?.Remove(follow);
            shop.Followers?.Remove(follow);
            await userRepository.SaveAsync();

            response.IsSuccess = true;
            response.Data = $"You unfollow {shop.Name}";
            return response;
        }

        public async Task<Generalresponse> UnfollowUserAsync(ApplicationUser applicationUser, string followedId)
        {
            Generalresponse response = new Generalresponse();
            if (applicationUser.Id == followedId)
            {
                response.IsSuccess = false;
                response.Data = "You Can't Follow Yourself";
                return response;
            }

            var user = await userRepository.GetAsync(u => u.Id == applicationUser.Id, "Following");
            var followed = await userRepository.GetAsync(e => e.Id == followedId, "Followers");
            if (followed == null)
            {
                response.IsSuccess = false;
                response.Data = "Shop not found";
                return response;
            }

            var follow = await userRepository.GetFollow(applicationUser.Id, followedId);
            if (follow == null)
            {
                response.IsSuccess = false;
                response.Data = "You Didn't Follow This account";
                return response;
            }

            userRepository.RemoveFollow(follow);
            user?.Following?.Remove(follow);
            followed.Followers?.Remove(follow);
            await userRepository.SaveAsync();

            response.IsSuccess = true;
            response.Data = $"You UnFollow {followed.FirstName} {followed.LastName}";
            return response;
        }

        public async Task<bool> IsFollowingGymAsync(string userId, int gymId)
        {
            var follow = await userRepository.GetGymFollow(userId, gymId);
            if (follow == null)
                return false;
            return true;
        }

        public async Task<bool> IsFollowingShopAsync(string userId, int shopId)
        {
            var follow = await userRepository.GetShopFollow(userId, shopId);
            if (follow == null)
                return false;
            return true;
        }

        public async Task<bool> IsFollowingUserAsync(string userId, string followedId)
        {
            if (userId == followedId)
                return false;

            var follow = await userRepository.GetFollow(userId, followedId);
            if (follow == null)
                return false;
            return true;
        }
    }
}
