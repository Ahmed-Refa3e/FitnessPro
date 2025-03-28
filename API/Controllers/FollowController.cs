using Core.DTOs.GeneralDTO;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FollowController(IFollowService service, UserManager<ApplicationUser> userManager) : BaseApiController
    {
        [HttpPost("follow-gym/{gymId}")]
        public async Task<IActionResult> FollowGym(int gymId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await service.FollowGymAsync(user, gymId);

            return HandleResponse(result);
        }

        [HttpPost("follow-shop/{shopId}")]
        public async Task<IActionResult> FollowShop(int shopId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await service.FollowShopAsync(user, shopId);

            return HandleResponse(result);
        }

        [HttpPost("follow-user/{followedId}")]
        public async Task<IActionResult> FollowUser(string followedId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await service.FollowUserAsync(user, followedId);

            return HandleResponse(result);
        }

        [HttpDelete("unfollow-gym/{gymId}")]
        public async Task<IActionResult> UnfollowGym(int gymId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await service.UnfollowGymAsync(user, gymId);

            return HandleResponse(result);
        }

        [HttpDelete("unfollow-shop/{shopId}")]
        public async Task<IActionResult> UnfollowShop(int shopId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await service.UnfollowShopAsync(user, shopId);

            return HandleResponse(result);
        }

        [HttpDelete("unfollow-user/{followedId}")]
        public async Task<IActionResult> UnfollowUser(string followedId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await service.UnfollowUserAsync(user, followedId);

            return HandleResponse(result);
        }

        [HttpGet("is-following-gym/{gymId}")]
        public async Task<IActionResult> IsFollowingGym(int gymId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            bool isFollowing = await service.IsFollowingGymAsync(user.Id, gymId);
            return Ok(new { isFollowing });
        }

        [HttpGet("is-following-shop/{shopId}")]
        public async Task<IActionResult> IsFollowingShop(int shopId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            bool isFollowing = await service.IsFollowingShopAsync(user.Id, shopId);
            return Ok(new { isFollowing });
        }

        [HttpGet("is-following-user/{followedId}")]
        public async Task<IActionResult> IsFollowingUser(string followedId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            bool isFollowing = await service.IsFollowingUserAsync(user.Id, followedId);
            return Ok(new { isFollowing });
        }
        private IActionResult HandleResponse(Generalresponse result)
        {
            if (result.IsSuccess)
                return Created("", new { message = result.Data });

            return result.Data switch
            {
                "You already follow this gym" => Conflict(result),
                "You already follow this Shop" => Conflict(result),
                "You Can't Follow Yourself" => Forbid(),
                "Gym not found" => NotFound(result),
                "Shop not found" => NotFound(result),
                "User not found" => NotFound(result),
                _ => BadRequest(result)
            };
        }
    }
}
