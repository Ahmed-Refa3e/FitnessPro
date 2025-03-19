using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class AddLikeDTO
    {
        [Required]
        public int OwnerId { get; set; }
        [Required]
        public string UserId {  get; set; }
        [Required]
        public string Type {  get; set; }
    }
}
