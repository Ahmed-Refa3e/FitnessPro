using Core.Enums;

namespace Infrastructure.SeedData.SeedingHelpers
{
    public class SeedingTraining
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public TrainingType TrainingType { get; set; }
        public decimal Price { get; set; }
        public int NoOfSessionsPerWeek { get; set; }
        public int DurationOfSession { get; set; }
        public required string CoachEmail { get; set; }
    }
}
