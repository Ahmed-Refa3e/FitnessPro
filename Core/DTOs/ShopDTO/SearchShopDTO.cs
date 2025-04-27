using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ShopDTO
{
    public class SearchShopDTO
    {
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool OrderedByTheMostFollowerNumber { get; set; } = false;
    }
}
