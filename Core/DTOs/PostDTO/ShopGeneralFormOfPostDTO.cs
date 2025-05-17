using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class ShowGeneralFormOfPostDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public dynamic CreatedAt { get; set; }
        public string PhotoPass { get; set; }
        public string Name { get; set; }
        public bool IsYourPost { get; set; }
        public bool IsLikedByYou { get; set; }
        public string LikeType { get; set; }
        public PageType SourceType { get; set; }
        public dynamic SourceId { get; set; }
        public List<string>? PictureUrls { get; set; }
        public LikesDetailsDTO LikesDetails { get; set; }
    }
}
