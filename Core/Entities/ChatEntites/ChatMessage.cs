using Core.Entities.Identity;

namespace Core.Entities.ChatEntites
{
    public class ChatMessage
    {
        public int id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public required string SenderId { get; set; }
        public required string ReceiverId { get; set; }
        public ApplicationUser? receiver { get; set; }
        public ApplicationUser? sender { get; set; }
        public DateTime timeStamp { get; set; } = DateTime.Now;

        public bool IsSeen { get; set; } = false;
        public DateTime? SeenAt { get; set; }
    }
}
