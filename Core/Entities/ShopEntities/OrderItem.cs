using Microsoft.EntityFrameworkCore;

namespace Core.Entities.ShopEntities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        [Precision(18, 4)]
        public decimal Price { get; set; } // Price at the time of purchase
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
