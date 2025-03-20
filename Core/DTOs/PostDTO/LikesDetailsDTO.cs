using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class LikesDetailsDTO
    {
        public int Count { get; set; } = 0;
        public List<string> OrderedType { get; set; }=new List<string>();
    }
}
