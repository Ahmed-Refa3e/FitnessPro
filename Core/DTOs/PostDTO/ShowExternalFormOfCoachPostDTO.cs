using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class ShowExternalFormOfCoachPostDTO: ShowExternalFormOfPostDTO
    {
        public string CoachId { get; set; }
    }
}
