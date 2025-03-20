using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class AddCommentDTO
    {
        [Required]
        public int OwnerId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Content { get; set; }
    }
}
