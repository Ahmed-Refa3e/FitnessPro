using Core.Entities.Identity;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.PostEntities
{
    public class Like
    {
        public int Id { get; set; }
        public LikeType Type { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
