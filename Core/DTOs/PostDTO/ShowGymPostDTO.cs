using Core.Entities.PostEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class ShowGymPostDTO:ShowPostDTO
    {
        public int GymId { get; set; }
    }
}
