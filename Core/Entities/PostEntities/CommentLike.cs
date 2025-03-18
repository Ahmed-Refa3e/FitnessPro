using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.PostEntities
{
    public class CommentLike : Like
    {
        public int? CommentId { get; set; }
        public Comment? Comment { get; set; }
    }
}
