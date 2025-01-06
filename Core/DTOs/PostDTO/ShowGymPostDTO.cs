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
        public ShowGymPostDTO()
        {
            
        }
        public ShowGymPostDTO(GymPost post)
        {
            this.Id = post.Id;
            this.Content = post.Content;
            this.CreatedAt = post.CreatedAt;
            this.PhotoPass = post.Gym.PictureUrl;
            this.Name = post.Gym.GymName;
            this.GymId = post.GymId;
            this.PictureUrls = post.PictureUrls.Select(x => x.Url).ToList();
        }
    }
}
