using Core.Entities.Identity;

namespace Core.Entities.ChatEntites
{
    public class UserConnection
    {
        public string iD { get; set; } = Guid.NewGuid().ToString();
        public required string connectionId { get; set; }
        public required string userId { get; set; }
        public ApplicationUser? user { get; set; }
        public bool isOnline { get; set; }
    }
}
