using Core.DTOs.GeneralDTO;
using Core.Interfaces.Repositories;
using Core.Interfaces.Repositories.ChatRepositories;
using Core.Interfaces.Services;

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
        public async Task<Generalresponse> GetChatHistoryAsync(string FirstUserId, string LastUserId)
        {
            var messages = await chatRepository.GetChatHistoryAsync(FirstUserId, LastUserId);
            return new Generalresponse
            {
                IsSuccess = true,
                Data = messages
            };
        }

        public async Task<Generalresponse> GetContactsAsync(string UserId)
        {
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

            return new Generalresponse
            {
                IsSuccess = true,
                Data = contacts
            };
        }

        public async Task<Generalresponse> GetUserStatusAsync(string UserId)
        {
            var IsOnline = await connectionRepository.IsUserOnlineAsync(UserId);
            return new Generalresponse
            {
                IsSuccess = true,
                Data = new { userId = UserId, isOnline = IsOnline }
            };
        }
    }
}
