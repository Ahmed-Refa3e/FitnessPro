using Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.ShopEntities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public int Quantity { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int ShopId { get; set; }
        public Shop Shop { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
