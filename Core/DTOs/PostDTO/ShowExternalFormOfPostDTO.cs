using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class ShowExternalFormOfPostDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public dynamic CreatedAt { get; set; }
        public string PhotoPass { get; set; }
        public string Name { get; set; }
        public List<string>? PictureUrls { get; set; }
        public LikesDetailsDTO LikesDetails { get; set; }
    }
}
