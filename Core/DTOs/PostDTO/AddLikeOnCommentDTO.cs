using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class AddLikeOnCommentDTO
    {
        [Required]
        public int CommentId { get; set; }
        [Required]
        public string Type { get; set; }
    }
}
