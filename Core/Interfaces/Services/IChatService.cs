using Core.DTOs.GeneralDTO;

namespace Core.Interfaces.Services
{
    public interface IChatService
    {
        Task<Generalresponse> GetChatHistoryAsync(string FirstUserId, string LastUserId);
        Task<Generalresponse> GetContactsAsync(string UserId);
        Task<Generalresponse> GetUserStatusAsync(string UserId);
    }
}
