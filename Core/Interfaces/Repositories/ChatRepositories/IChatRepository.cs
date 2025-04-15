using Core.Entities.ChatEntites;

namespace Core.Interfaces.Repositories.ChatRepositories
{
    public interface IChatRepository : IGenericRepository<ChatMessage>
    {
        Task<List<ChatMessage>> GetChatHistoryAsync(string userId1, string userId2);
        Task MarkMessagesAsSeenAsync(string senderId, string receiverId);
    }
}
