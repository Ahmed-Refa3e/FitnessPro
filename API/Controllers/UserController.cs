using Core.DTOs.UserDTO;
using Core.Entities.ShopEntities;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Repositories.ShopRepositories;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly IUserService service;
        private readonly IUserRepository userRepository;
        private readonly IGymRepository gymRepository;
        private readonly FitnessContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public UserController(IUserService service,
            IUserRepository userRepository,IGymRepository gymRepository,FitnessContext context
                    ,UserManager<ApplicationUser> userManager)
        {
            this.service = service;
            this.userRepository = userRepository;
            this.gymRepository = gymRepository;
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet("GetAllCoaches")]
        public async Task<IActionResult> GetAllCoaches()
        {
            var result = await service.GetAllCoachesAsync();
            return Ok(result);
        }

        [HttpGet("CoachDetails/{CoachId}")]
        public async Task<IActionResult> GetCoachDetails(string CoachId)
        {
            var result = await service.GetCoachDetailsAsync(CoachId);

            if (result.IsSuccess)
                return Ok(result);
            else
                return NotFound(result);
        }

        [HttpGet("TraineeDetails/{TraineeId}")]
        public async Task<IActionResult> GetTraineeDetails(string TraineeId)
        {
            var result = await service.GetTraineeDetailsAsync(TraineeId);

            if (result.IsSuccess)
                return Ok(result);
            else
                return NotFound(result);
        }

        [Authorize]
        [HttpPatch("UpdateProfileDetails")]
        public async Task<IActionResult> UpdateProfileDetails(UpdateProfileDTO model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();
            try
            {
                var result = await service.UpdateProfileDetailsAsync(model, userId);
                if(result.IsSuccess)
                    return Ok(result);
                return BadRequest(result);
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest("User Not Found");
            }
        }

        [Authorize]
        [HttpPost("UpdateProfilePicture")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePictureDTO pictureDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            try
            {
                var result = await service.ChangeProfilePictureAsync(pictureDTO.ProfilePicture, userId);
                if (result.IsSuccess)
                    return Ok(result);
                return BadRequest(result);
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest("User Not Found");
            }
        }
        [Authorize]
        [HttpPost("DeleteProfilePicture")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();
            var result = await service.DeleteProfilePictureAsync(user);
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [Authorize]
        [HttpPut("SetOnlineAvailability")]
        public async Task<IActionResult> SetOnlineAvailability(bool isAvailable)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            try
            {
                var result = await service.SetOnlineAvailabilityAsync(userId, isAvailable);

                if (result.IsSuccess)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPost("FollowUser/{followedId}")]
        public async Task<IActionResult> FollowUser(string followedId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            if (userId == followedId)
                return BadRequest("You can't Follow Yourself");

            var user = await userRepository.GetAsync(e => e.Id == userId, "Following");
            var followedUser = await userRepository.GetAsync(e => e.Id == followedId, "Followers");

            if (followedUser == null)
                return BadRequest("User Not Found");

            var checkFollow = await userRepository.GetFollow(userId:userId,FollowedId:followedId);
            if (checkFollow != null)
            {
                return BadRequest($"You Follow {followedUser.FirstName} {followedUser.LastName} already");
            }

            var follow = new UserFollow
            {
                FollowerId = userId,
                FollowingId = followedId,
                FollowerUser = user,
                FollowingUser = followedUser
            };

            await userRepository.AddFollower(follow);
            user?.Following?.Add(follow);
            followedUser.Followers?.Add(follow);

            await userRepository.SaveAsync();
            return Ok($"You Now Follow {followedUser.FirstName} {followedUser.LastName}");
        }
        [Authorize]
        [HttpDelete("UnfollowUser/{followedId}")]
        public async Task<IActionResult> UnfollowUser(string followedId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Unauthorized();

            if (userId == followedId)
                return BadRequest("You can't follow yourself");

            var user = await userRepository.GetAsync(u => u.Id == userId, "Following");
            var followed = await userRepository.GetAsync(e => e.Id == followedId, "Followers");
            if (followed == null)
                return BadRequest("User Not Found");
            
            var follow = await userRepository.GetFollow(userId, followedId);
            if (follow == null)
                return BadRequest("You Didn't follow this account");

            userRepository.RemoveFollow(follow);
            user?.Following?.Remove(follow);
            followed.Followers?.Remove(follow);
            return Ok($"You UnFollow {followed.FirstName} {followed.LastName}");
        }


        [Authorize]
        [HttpPost("FollowGym/{GymId}")]
        public async Task<IActionResult> FollowGym(int GymId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var checkfollow = await userRepository.GetGymFollow(userId, GymId);
            if (checkfollow != null)
                return BadRequest("You follow this gym already");

            var user = await userRepository.GetAsync(e => e.Id == userId, "FollowedGyms");
            var gym = await gymRepository.GetByIdAsync(GymId);
            if (gym == null) return BadRequest("Gym Not Found");

            var follow = new GymFollow
            {
                GymId = GymId,
                FollowerId = userId,
                FollowerUser = user,
                Gym = gym,
            };

            await userRepository.AddGymFollower(follow);
            gym.Followers?.Add(follow);
            user?.FollowedGyms?.Add(follow);
            await userRepository.SaveAsync();

            return Ok($"You Now Follow {gym.GymName}..");
        }

        [Authorize]
        [HttpDelete("UnFollowGym/{GymId}")]
        public async Task<IActionResult> UnFollowGym(int GymId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var follow = await userRepository.GetGymFollow(userId, GymId);
            if (follow == null)
                return BadRequest("You Didn't Follow This Gym");

            var user = await userRepository.GetAsync(e => e.Id == userId, "FollowedGyms");
            var gym = await gymRepository.GetByIdAsync(GymId);
            if (gym == null) return BadRequest("Gym Not Found");

            userRepository.RemoveGymFollow(follow);
            user?.FollowedGyms?.Remove(follow);
            gym.Followers?.Remove(follow);
            await userRepository.SaveAsync();

            return Ok($"You unfollow {gym.GymName}");
        }

        [Authorize]
        [HttpPost("FollowShop/{shopId}")]
        public async Task<IActionResult> FollowShop(int shopId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var checkfollow = await userRepository.GetShopFollow(userId, shopId);
            if (checkfollow != null)
                return BadRequest("You follow this shop already");

            var user = await userRepository.GetAsync(e => e.Id == userId, "FollowedShops");
            var shop = context.Shops.Include(e=>e.Followers).FirstOrDefault(e=>e.Id == shopId);  
            if (shop == null) return BadRequest("shop Not Found");

            var follow = new ShopFollow
            {
                ShopId = shopId,
                FollowerId = userId,
                FollowerUser = user,
                Shop = shop,
            };

            await userRepository.AddShopFollower(follow);
            shop.Followers?.Add(follow);
            user?.FollowedShops?.Add(follow);
            await userRepository.SaveAsync();

            return Ok($"You Now Follow {shop.Name}..");
        }

        [Authorize]
        [HttpDelete("UnFollowShop/{ShopId}")]
        public async Task<IActionResult> UnFollowShop(int ShopId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var follow = await userRepository.GetShopFollow(userId, ShopId);
            if (follow == null)
                return BadRequest("You Didn't Follow This shop");

            var user = await userRepository.GetAsync(e => e.Id == userId, "FollowedShops");
            var shop = context.Shops.Include(e => e.Followers).FirstOrDefault(e => e.Id == ShopId);
            if (shop == null) return BadRequest("shop Not Found");

            userRepository.RemoveShopFollow(follow);
            user?.FollowedShops?.Remove(follow);
            shop.Followers?.Remove(follow);
            await userRepository.SaveAsync();

            return Ok($"You unfollow {shop.Name}");
        }
    }
}
