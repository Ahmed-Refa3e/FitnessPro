using Core.Entities.ChatEntites;

namespace Core.DTOs.ChatDTO
{
    public class PaginatedMessagesDto
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public List<ChatMessage> Messages { get; set; }
    }
}
