using Microsoft.EntityFrameworkCore;

namespace Core.DTOs.PostDTO
{
    [Keyless]
    public class RawPostDTO
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CoachId { get; set; }
        public int? GymId { get; set; }
        public int? ShopId { get; set; }
        public string? PhotoPass { get; set; }
        public string? EntityName { get; set; }
        public string? SourceType { get; set; }
        public bool IsYourPost { get; set; }
    }
}
