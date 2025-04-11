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
        // just Group or private
        [RegularExpression("Group|Private", ErrorMessage = "TrainingType must be either 'Group' or 'Private'")]
        public required string TrainingType { get; set; } // Group or private  
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int NoOfSessionsPerWeek { get; set; }
        [Required]
        public int DurationOfSession { get; set; } // Duration in minutes     
    }
}
