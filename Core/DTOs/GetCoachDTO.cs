using Core.Entities.OnlineTrainingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
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
