using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ShopDTO.ProductDTO
{
    public class ProductSearchDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Name {  get; set; }
        public long? MinimumPrice {  get; set; }
        public long? MaximumPrice { get; set; }
        public int? ShopID {  get; set; }
        public int? CategoryID { get; set; }
        public bool SearchByBiggetDiscount { get; set; }
        public bool SearchByPriceDescending { get; set; }
        public bool SearchByPriceAscending { get; set; }

    }
}
