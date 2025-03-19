using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class ShowCommentDTO
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string PictureUrl { get; set; }
        public string Content { get; set; }
        public dynamic Date { get; set; }
        public bool HaveComments {  get; set; }
    }
}
