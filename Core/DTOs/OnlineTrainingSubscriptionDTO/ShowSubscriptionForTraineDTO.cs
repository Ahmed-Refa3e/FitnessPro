using Core.Entities.Identity;
using Core.Entities.OnlineTrainingEntities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.OnlineTrainingSubscriptionDTO
{
    public class ShowSubscriptionForTraineDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } // Active, Completed, Cancelled
        public int? OnlineTrainingId { get; set; }
        public string OnlineTrainingTitle { get; set; }
        public string? CoachId { get; set; }
        public string? CoachName { get; set; }
    }
}
