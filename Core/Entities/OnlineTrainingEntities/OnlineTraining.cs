using Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.OnlineTrainingEntities
{
    public class OnlineTraining
    {
        [Key]
        public int TrainingID { get; set; }
        public required string CoachID { get; set; }
        public required Coach Coach { get; set; }
        public required string? Title { get; set; }
        public required string Description { get; set; }
        public required int NoOfSessionsPerWeek { get; set; }
        public int DurationOfSession { get; set; } // Duration in minutes
        public required string TrainingType { get; set; } // Private, Group, etc.

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
