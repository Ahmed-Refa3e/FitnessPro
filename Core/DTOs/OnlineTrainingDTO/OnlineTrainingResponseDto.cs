namespace Core.DTOs.OnlineTrainingDTO
{
    public class OnlineTrainingResponseDto
    {
        public int Id { get; set; }
        public string CoachID { get; set; } = null!;
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string TrainingType { get; set; } // Group or private
        public decimal Price { get; set; }
        public int NoOfSessionsPerWeek { get; set; }
        public int DurationOfSession { get; set; } // Duration in minutes
    }
}
