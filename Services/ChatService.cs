using Core.DTOs.ChatDTO;
using Core.DTOs.GeneralDTO;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Repositories.ChatRepositories;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository chatRepository;
        private readonly IUserConnectionRepository connectionRepository;
        private readonly IUserRepository userRepository;

        public ChatService(IChatRepository chatRepository, IUserConnectionRepository connectionRepository
            , IUserRepository userRepository)
        {
            this.chatRepository = chatRepository;
            this.connectionRepository = connectionRepository;
            this.userRepository = userRepository;
        }


        public async Task<GeneralResponse> GetChatHistoryAsync(string FirstUserId, string LastUserId
            , int pageNumber, int pageSize)
        {
            var messages = await chatRepository.GetChatHistoryAsync(FirstUserId, LastUserId, pageNumber, pageSize);

            List<MessageResponseDTO> returnedMessages = new List<MessageResponseDTO>();
            foreach (var message in messages.Messages)
            {
                var newmessage = new MessageResponseDTO
                {
                    id = message.id,
                    Content = message.Content,
                    ImageUrl = message.ImageUrl,
                    SeenAt = message.SeenAt,
                    SenderId = message.SenderId,
                    IsSeen = message.IsSeen,
                    timeStamp = message.timeStamp,
                    ReceiverId = message.ReceiverId
                };
                returnedMessages.Add(newmessage);
            }
            return new GeneralResponse
            {
                IsSuccess = true,
                Data = new PagedResult<MessageResponseDTO>
                        (returnedMessages, messages.TotalCount, messages.PageNumber, messages.PageSize)
            };
        }

        public async Task<GeneralResponse> GetContactsAsync(string UserId)
        {
            var userfromdb = await userRepository.GetAsync(e=>e.Id == UserId);
            if (userfromdb == null)
            {
                return new GeneralResponse
                {
                    Data = "User Not Found",
                    IsSuccess = false,
                };
            }

            var messages = await chatRepository.ExecuteQueryAsync(
               chatRepository.GetQueryable().Where(m => m.ReceiverId == UserId || m.SenderId == UserId)
            );

            var contactIds = messages
                .Select(m => m.SenderId == UserId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToList();

            var contactUsers = userRepository
                .GetAll(u => contactIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.ProfilePictureUrl
                })
                .ToList();

            var contacts = contactIds.Select(id =>
            {
                var lastMessage = messages
                    .Where(m => m.SenderId == id || m.ReceiverId == id)
                    .OrderByDescending(m => m.timeStamp)
                    .FirstOrDefault();

                var user = contactUsers.FirstOrDefault(u => u.Id == id);

                return new
                {
                    userId = id,
                    LastMessage = lastMessage?.Content,
                    User = user
                };
            }).ToList();

            return new GeneralResponse
            {
                IsSuccess = true,
                Data = contacts
            };
        }

        public async Task<GeneralResponse> GetUserStatusAsync(string UserId)
        {
            var IsOnline = await connectionRepository.IsUserOnlineAsync(UserId);
            return new GeneralResponse
            {
                IsSuccess = true,
                Data = new { userId = UserId, isOnline = IsOnline }
            };
        }

        public async Task<int> GetAllUnreadMessagesAsync(string UserId)
        {
            var messagesCount = await
                chatRepository.GetQueryable().Where(e => e.ReceiverId == UserId && e.IsSeen == false)
                .CountAsync();
            return messagesCount;
        }

        public async Task<int> GetUnreadMessagesWithAnotherUserAsync(string UserId, string senderId)
        {
            var messagesCount = await chatRepository.GetQueryable().Where(e => e.SenderId == senderId
                && e.ReceiverId == UserId && e.IsSeen == false).CountAsync();
            return messagesCount;
        }
    }
}
