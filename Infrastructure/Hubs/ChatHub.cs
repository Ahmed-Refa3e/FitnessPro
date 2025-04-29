using Core.Entities.ChatEntites;
using Core.Entities.Identity;
using Core.Interfaces.Repositories.ChatRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IChatRepository chatRepository;
        private readonly IUserConnectionRepository connectionRepository;

        public ChatHub(UserManager<ApplicationUser> userManager, IChatRepository chatRepository
            , IUserConnectionRepository connectionRepository)
        {
            this.userManager = userManager;
            this.chatRepository = chatRepository;
            this.connectionRepository = connectionRepository;
        }

        public async Task SendMessage(string receiverId, string message,string? imageUrl)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User is not authenticated.");

            var chatMessage = new ChatMessage
            {
                ReceiverId = receiverId,
                SenderId = userId,
                Content = message,
                ImageUrl = imageUrl
            };
            chatRepository.Add(chatMessage);
            await chatRepository.SaveChangesAsync();

            await Clients.Users(receiverId, userId).SendAsync("ReceiveMessage", new
            {
                SenderId = userId,
                ReceiverId = receiverId,
                Content = message,
                ImageUrl = imageUrl,
                TimeStamp = chatMessage.timeStamp
            });
        }

        public async Task MarkMessagesAsSeen(string senderId)
        {
            var receiverId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(receiverId))
                throw new HubException("User is not authenticated.");

            await chatRepository.MarkMessagesAsSeenAsync(senderId, receiverId);
            await chatRepository.SaveChangesAsync();
            await Clients.User(senderId).SendAsync("MessagesSeen", receiverId);
        }

        public async Task TypingStatus(string receiverId, bool isTyping)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
                throw new HubException("User is not authenticated.");

            await Clients.User(receiverId).SendAsync("TypingStatusChanged", isTyping);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var newConnection = new UserConnection
                {
                    isOnline = true,
                    userId = userId,
                    connectionId = Context.ConnectionId
                };

                connectionRepository.Add(newConnection);
                await connectionRepository.SaveChangesAsync();

                await Clients.All.SendAsync("UserStatusChanged", userId, true);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var userConnections = connectionRepository.GetQueryable()
                    .Where(e => e.userId == userId).ToList();

                await connectionRepository.RemoveConnectionAsync(userConnections, Context.ConnectionId);

                if (!userConnections.Any(c => c.connectionId != Context.ConnectionId))
                {
                    await Clients.All.SendAsync("UserStatusChanged", userId, false);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
