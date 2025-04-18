using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class ChatController(IChatService service) : BaseApiController
    {
        [HttpGet("history/{ohterUserId}")]
        public async Task<IActionResult> GetChatHistory(string ohterUserId, int pageNumber = 1, int pageSize = 20)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return Unauthorized();

            var result = await service.GetChatHistoryAsync(currentUserId, ohterUserId, pageNumber, pageSize);

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

        [HttpGet("unread-messages/count")]
        public async Task<IActionResult> GetAllUnreadMessagges()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var count = await service.GetAllUnreadMessaggesAsync(userId);
            return Ok(count);
        }

        [HttpGet("unread-messages/{senderId}/count")]
        public async Task<IActionResult> GetUnreadMessagesWithAnotherUser(string senderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var messagesCount = await service.GetUnreadMessagesWithAnotherUserAsync(userId, senderId);
            return Ok(messagesCount);
        }
    }
}
