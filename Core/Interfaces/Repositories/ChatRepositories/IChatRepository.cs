using Core.DTOs.ChatDTO;
using Core.Entities.ChatEntites;

namespace Core.Interfaces.Repositories.ChatRepositories
{
    public interface IChatRepository : IGenericRepository<ChatMessage>
    {
        Task<PaginatedMessagesDto> GetChatHistoryAsync(string userId1, string userId2, int pageNumber, int pageSize);
        Task MarkMessagesAsSeenAsync(string senderId, string receiverId);
    }
}
