using Core.Entities.ChatEntites;
using Core.Interfaces.Repositories.ChatRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.ChatRepositories
{
    public class ChatRepository : GenericRepository<ChatMessage>, IChatRepository
    {
        private readonly FitnessContext context;

        public ChatRepository(FitnessContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<List<ChatMessage>> GetChatHistoryAsync(string userId1, string userId2)
        {
            return await context.messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.timeStamp)
                .ToListAsync();
        }

        public async Task MarkMessagesAsSeenAsync(string senderId, string receiverId)
        {
            var messages = await context.messages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsSeen)
                .ToListAsync();

            foreach (var msg in messages)
            {
                msg.IsSeen = true;
                msg.SeenAt = DateTime.UtcNow;
            }
        }
    }
}
