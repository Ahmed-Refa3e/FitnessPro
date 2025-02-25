using Core.Entities.GymEntities;
using Core.Entities.Identity;
using Core.Entities.OnlineTrainingEntities;
using Infrastructure.SeedData.SeedingHelpers;

namespace Infrastructure.SeedData
{
    public class SeedingData
    {
        public List<Coach> Coaches { get; set; } = new();
        public List<Trainee> Trainees { get; set; } = new();
        public List<SeedingGym> Gyms { get; set; } = new();
        public List<SeedingTraining> OnlineTrainings { get; set; } = new();
    }
}
