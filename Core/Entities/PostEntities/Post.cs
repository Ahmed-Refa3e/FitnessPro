using Core.DTOs.PostDTO;
using Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.PostEntities
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Post()
        {
            
        }
        public Post(AddPostDTO post)
        {
            this.Content = post.Content;
            this.CreatedAt= DateTime.Now;
        }
    }
}
