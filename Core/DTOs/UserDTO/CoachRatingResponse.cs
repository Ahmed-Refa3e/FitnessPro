using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UserDTO
{
    public class CoachRatingResponse
    {
        public int CoachRatingId { get; set; }
        public required string CoachId { get; set; }
        public string? TraineeId { get; set; }
        public int ratingValue { get; set; }
        public string? Review { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
