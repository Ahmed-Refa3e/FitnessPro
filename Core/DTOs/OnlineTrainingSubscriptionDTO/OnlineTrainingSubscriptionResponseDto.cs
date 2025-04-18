namespace Core.DTOs.OnlineTrainingSubscriptionDTO
{
    public class OnlineTrainingSubscriptionResponseDto
    {
        public int Id { get; set; }
        public string TraineeID { get; set; } = null!;
        public int OnlineTrainingId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? OnlineTrainingTitle { get; set; } // Optional, can be null if not needed
        public string? TraineeName { get; set; } // Optional, can be null if not needed
        public bool IsActive { get; set; }
    }
}
