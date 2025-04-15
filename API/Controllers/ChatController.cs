using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class ChatController(IChatService service) : BaseApiController
    {
        [HttpGet("history/{userId1}/{userId2}")]
        public async Task<IActionResult> GetChatHistory(string userId1, string userId2)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != userId1 && currentUserId != userId2)
                return Forbid();

            var result = await service.GetChatHistoryAsync(userId1, userId2);

            return Ok(result);
        }

        [HttpGet("contacts")]
        public async Task<IActionResult> GetContacts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await service.GetContactsAsync(userId);
            return Ok(result);
        }

        [HttpGet("user-status/{userId}")]
        public async Task<IActionResult> GetUserStatus(string userId)
        {
            var result = await service.GetUserStatusAsync(userId);
            return Ok(result);
        }
    }
}
