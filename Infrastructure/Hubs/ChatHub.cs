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

        public async Task SendMessage(string receiverId, string message)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                throw new HubException("User is not authenticated.");

            var ChatMessage = new ChatMessage
            {
                ReceiverId = receiverId,
                SenderId = userId,
                Content = message
            };
            chatRepository.Add(ChatMessage);
            await chatRepository.SaveChangesAsync();

            await Clients.Users(receiverId, userId).SendAsync("ReceiveMessage", new
            {
                SenderId = userId,
                ReceiverId = receiverId,
                Content = message,
                TimeStamp = ChatMessage.timeStamp
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

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var oldConnections = connectionRepository.GetQueryable()
                    .Where(c => c.userId == userId);
                foreach (var conn in oldConnections)
                    connectionRepository.Delete(conn);

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
                await connectionRepository.RemoveConnectionAsync(userId, Context.ConnectionId);
                await Clients.All.SendAsync("UserStatusChanged", userId, false);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
