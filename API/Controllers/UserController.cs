using Core.DTOs.UserDTO;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService service, UserManager<ApplicationUser> userManager) : BaseApiController
    {
        [HttpGet("GetAllCoaches")]
        public async Task<IActionResult> GetAllCoaches([FromQuery] GetCoachesDTO getCoachesDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await service.GetAllCoachesAsync(getCoachesDTO);
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

        [Authorize]
        [HttpGet("ProfileDetails")]
        public async Task<IActionResult> GetProfileDetails()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var result = service.GetProfileDetails(user);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("UpdateProfileDetails")]
        public async Task<IActionResult> UpdateProfileDetails(UpdateProfileDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var result = await service.UpdateProfileDetailsAsync(model, user);
            if (result.IsSuccess)
                return Ok(result);
            return StatusCode(422, result);

        }

        [Authorize]
        [HttpPost("UpdateProfilePicture")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePictureDTO pictureDTO)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            if (pictureDTO.ProfilePicture == null || pictureDTO.ProfilePicture.Length == 0)
                return BadRequest(new { Message = "Invalid file. Please upload a valid image." });

            var result = await service.ChangeProfilePictureAsync(pictureDTO.ProfilePicture, user);

            if (result.IsSuccess)
                return Ok(new { Message = "Profile picture updated successfully" });
            return StatusCode(422, result);
        }

        [Authorize]
        [HttpPost("DeleteProfilePicture")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { Message = "User is not authorized or does not exist" });

            var result = service.DeleteProfilePictureAsync(user);

            if (result.IsSuccess)
            {
                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return StatusCode(500, new { Message = "Profile picture deleted, but user update failed." });

                return Ok(new { Message = "Your profile picture has been removed" });
            }

            return NotFound(result);
        }
    }
}
