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
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<PostPictureUrl>? PictureUrls { get; set; }= new List<PostPictureUrl>();
        public List<PostLike>? Likes { get; set; }=new List<PostLike>();
        public List<PostComment>? Comments { get; set; } = new List<PostComment>();
    }
}
