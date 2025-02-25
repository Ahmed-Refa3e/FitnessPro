using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.OnlineTrainingDTO
{
    public class CreateOnlineTrainingDTO
    {
        [Required]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        public required TrainingType TrainingType { get; set; } // Group or private
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int NoOfSessionsPerWeek { get; set; }
        [Required]
        public int DurationOfSession { get; set; } // Duration in minutes   
    }
}
