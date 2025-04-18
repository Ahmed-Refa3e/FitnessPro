using Core.DTOs.GeneralDTO;

namespace Core.Interfaces.Services
{
    public interface IChatService
    {
        Task<Generalresponse> GetChatHistoryAsync(string FirstUserId, string LastUserId, int pageNumber, int pageSize);
        Task<Generalresponse> GetContactsAsync(string UserId);
        Task<Generalresponse> GetUserStatusAsync(string UserId);
        Task<int> GetAllUnreadMessaggesAsync(string UserId);
        Task<int> GetUnreadMessagesWithAnotherUserAsync(string UserId, string senderId);
    }
}
