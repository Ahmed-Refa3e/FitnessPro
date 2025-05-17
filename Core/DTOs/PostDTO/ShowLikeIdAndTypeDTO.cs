using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class ShowLikePostIdAndTypeDTO
    {
        public int PostId { get; set; }
        public LikeType Type { get; set; }
    }
}
