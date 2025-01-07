using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.PostEntities
{
    public class PostPictureUrl
    {
        public int Id { get; set; }
        public string Url {  get; set; }
        public int PostId {  get; set; }
        public Post Post { get; set; }
    }
}
