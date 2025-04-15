using Core.DTOs.ChatDTO;
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
        public async Task<PaginatedMessagesDto> GetChatHistoryAsync(string userId1, string userId2, int pageNumber, int pageSize)
        {
            var query = context.messages
                .AsNoTracking()
                .Where(e => (e.ReceiverId == userId1 && e.SenderId == userId2) ||
                                     (e.SenderId == userId1 && e.ReceiverId == userId2))
                .OrderBy(e => e.timeStamp);

            var totalCount = await query.CountAsync();

            var paginatedMessages = await query
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize).ToListAsync();

            return new PaginatedMessagesDto
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                PageNumber = pageNumber,
                Messages = paginatedMessages
            };
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
