using Core.DTOs.OnlineTrainingDTO;

namespace Core.DTOs.UserDTO
{
    public class GetCoachDetailsDTO
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string? ProfilePictureUrl { get; set; }
        public required string? Bio { get; set; }
        public required string Gender { get; set; }
        public DateTime JoinedDate { get; set; }
        public List<GetOnlineTrainingDTO> OnlineTrainings { get; set; } = new();
    }
}
