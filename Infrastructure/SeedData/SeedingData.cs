using Core.Entities.Identity;
using Infrastructure.SeedData.SeedingHelpers;

namespace Infrastructure.SeedData
{
    public class SeedingData
    {
        public List<Coach> Coaches { get; set; } = new();
        public List<Trainee> Trainees { get; set; } = new();
        public List<SeedingGym> Gyms { get; set; } = new();
        public List<SeedingTraining> OnlineTrainings { get; set; } = new();
        public List<SeedingShop> Shops { get; set; } = new();
        public List<SeedingProduct> Products { get; set; } = new();
        public List<SeedingPost> Posts { get; set; } = new();
    }
}
