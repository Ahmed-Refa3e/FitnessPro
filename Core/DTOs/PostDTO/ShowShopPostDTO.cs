using Core.Entities.PostEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class ShowShopPostDTO:ShowPostDTO
    {
        public int ShopId { get; set;}
        public ShowShopPostDTO()
        {
            
        }
        public ShowShopPostDTO(ShopPost post)
        {
            this.Id = post.Id;
            this.Content = post.Content;
            this.CreatedAt = post.CreatedAt;
            this.PhotoPass = post.Shop.PictureUrl;
            this.Name = post.Shop.Name;
            this.ShopId = post.ShopId;
        }
    }
}
