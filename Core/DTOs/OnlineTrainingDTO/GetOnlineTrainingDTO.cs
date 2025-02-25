using Core.Entities.Identity;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.OnlineTrainingDTO
{
    public class GetOnlineTrainingDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TrainingType TrainingType { get; set; }
        public decimal Price { get; set; }
        public int NoOfSessionsPerWeek { get; set; }
        public int DurationOfSession { get; set; } 
    }
}
