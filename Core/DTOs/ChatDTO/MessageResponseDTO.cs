namespace Core.DTOs.ChatDTO
{
    public class MessageResponseDTO
    {
        public int id { get; set; }
        public required string Content { get; set; }
        public required string SenderId { get; set; }
        public required string ReceiverId { get; set; }
        public DateTime timeStamp { get; set; }

        public bool IsSeen { get; set; }
        public DateTime? SeenAt { get; set; }
    }
}
