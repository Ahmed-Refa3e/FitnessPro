using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class ShowCommentDTO
    {
        public string UserName { get; set; }
        public string PictureUrl { get; set; }
        public string Content { get; set; }
    }
}
