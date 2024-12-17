using Core.Entities.Identity;
using Core.Entities.OnlineTrainingEntities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.OnlineTrainingSubscriptionDTO
{
    public class AddSubscriptionDTO
    {
        [Required]
        public int OnlineTrainingId { get; set; }
        [Required]
        public string TraineeID { get; set; }
    }
}
