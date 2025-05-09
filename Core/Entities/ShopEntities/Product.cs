using Microsoft.EntityFrameworkCore;

namespace Core.Entities.ShopEntities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Precision(18, 4)]
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public int Quantity { get; set; }
        public List<Category>? Categories { get; set; } = new List<Category>();
        public int ShopId { get; set; }
        public Shop Shop { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
