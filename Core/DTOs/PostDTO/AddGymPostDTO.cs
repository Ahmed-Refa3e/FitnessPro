using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class AddGymPostDTO:AddPostDTO
    {
        [Required]
        public int GymId { get; set; }
    }
}
