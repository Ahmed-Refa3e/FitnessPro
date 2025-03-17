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
    }
}
