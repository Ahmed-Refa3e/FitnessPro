using Core.DTOs.GymDTO;
using Core.Entities.OnlineTrainingEntities;

namespace Core.DTOs.UserDTO
{
    public class GetCoachDTO
    {
        public required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string? ProfilePictureUrl { get; set; }
        public required string? Bio { get; set; }
        public required string Gender { get; set; }
        public DateTime JoinedDate { get; set; }
        public GymResponseDto? Gym { get; set; }
        public bool AvailableForOnlineTraining { get; set; }
        public IEnumerable<OnlineTraining>? OnlineTrainings { get; set; }
    }
}
