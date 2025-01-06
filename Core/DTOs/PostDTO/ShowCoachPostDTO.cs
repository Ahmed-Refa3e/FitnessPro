using Core.Entities.PostEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class ShowCoachPostDTO:ShowPostDTO
    {
        public string CoachId { get; set; }
        public ShowCoachPostDTO()
        {
            
        }
        public ShowCoachPostDTO(CoachPost post)
        {
            this.Id = post.Id;
            this.Content = post.Content;
            this.CreatedAt = post.CreatedAt;
            this.PhotoPass = post.Coach.ProfilePictureUrl;
            this.Name = post.Coach.FirstName + " " + post.Coach.FirstName;
            this.CoachId = post.CoachId;
            this.PictureUrls = post.PictureUrls.Select(x => x.Url).ToList();
        }
    }
}
