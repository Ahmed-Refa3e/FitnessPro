namespace Core.DTOs.OnlineTrainingDTO
{
    public class GetOnlineTrainingDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string TrainingType { get; set; }
        public decimal Price { get; set; }
        public int NoOfSessionsPerWeek { get; set; }
        public int DurationOfSession { get; set; }
    }
}
