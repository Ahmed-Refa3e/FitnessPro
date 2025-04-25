using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PostDTO
{
    public class ShowExternalFormOfShopPostDTO: ShowExternalFormOfPostDTO
    {
        public int ShopId {  get; set; }
    }
}
