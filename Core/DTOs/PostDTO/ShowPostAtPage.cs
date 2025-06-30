using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class ShowPostAtPage
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateOnly CreatedAt { get; set; }
        public List<string>? PictureUrls { get; set; }
    }
}
