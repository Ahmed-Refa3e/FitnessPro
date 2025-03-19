using Core.Entities.Identity;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.PostEntities
{
    public class PostLike : Like
    {
        public int? PostId { get; set; }
        public Post? Post { get; set; }
    }
}
