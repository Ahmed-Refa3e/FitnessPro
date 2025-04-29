namespace Core.DTOs.ChatDTO
{
    public class MessageResponseDTO
    {
        public int id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public required string SenderId { get; set; }
        public required string ReceiverId { get; set; }
        public DateTime timeStamp { get; set; }

        public bool IsSeen { get; set; }
        public DateTime? SeenAt { get; set; }
    }
}
