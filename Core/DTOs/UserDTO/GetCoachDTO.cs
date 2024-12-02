using Core.DTOs.GymDTO;
using Core.Entities.OnlineTrainingEntities;

namespace Core.DTOs.UserDTO
{
    public class GetCoachDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public string Gender { get; set; }
        public DateTime JoinedDate { get; set; }
        public GymResponseDto? Gym { get; set; }
        public bool AvailableForOnlineTraining { get; set; }
        public IEnumerable<OnlineTraining>? OnlineTrainings { get; set; }
    }
}
