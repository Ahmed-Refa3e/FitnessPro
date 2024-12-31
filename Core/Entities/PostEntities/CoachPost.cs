using Core.DTOs.PostDTO;
using Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.PostEntities
{
    public class CoachPost:Post
    {
        public string CoachId { get; set; }
        public Coach Coach { get; set; }
        public CoachPost()
        {
            
        }
        public CoachPost(AddCoachPostDTO post):base(post)
        {
            this.CoachId = post.CoachId;
        }
    }
}
