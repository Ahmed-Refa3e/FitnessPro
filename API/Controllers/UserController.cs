﻿using Core.DTOs.GeneralDTO;
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
            if (!result.IsSuccess)
            {
                return result.Data == "Users cannot update their bio."
                    ? BadRequest(result)
                    : StatusCode(422, result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("check-coach-business")]
        public async Task<IActionResult> CheckCoachBusinessAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User not found.");

            var response = await service.CheckUserStatusAsync(user);
            if (!response.IsSuccess)
                return Ok(response);

            return Ok(response);
        }

        [HttpGet("get-profile-picture/{UserId}")]
        public async Task<IActionResult> GetProfilePicture(string UserId)
        {
            var result = await service.GetProfilePictureAsync(UserId);
            if(result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }
    }
}
