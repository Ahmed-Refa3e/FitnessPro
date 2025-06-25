using Core.DTOs.GeneralDTO;

namespace Core.Interfaces.Services
{
    public interface IChatService
    {
        Task<GeneralResponse> GetChatHistoryAsync(string FirstUserId, string LastUserId, int pageNumber, int pageSize);
        Task<GeneralResponse> GetContactsAsync(string UserId);
        Task<GeneralResponse> GetUserStatusAsync(string UserId);
        Task<int> GetAllUnreadMessagesAsync(string UserId);
        Task<int> GetUnreadMessagesWithAnotherUserAsync(string UserId, string senderId);
    }
}
