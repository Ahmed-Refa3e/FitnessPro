using Core.DTOs.PostDTO;
using Core.Entities.ShopEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.PostEntities
{
    public class ShopPost:Post
    {
        public int ShopId {  get; set; }
        public Shop Shop { get; set; }
        public ShopPost()
        {
            
        }
        public ShopPost(AddShopPostDTO post) : base(post)
        {
            this.ShopId = post.ShopId;
        }
    }
}
