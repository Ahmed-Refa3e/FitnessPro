using Core.Entities.Identity;

namespace Core.Entities.ShopEntities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }
        public bool IsRecieved { get; set; } = false;
        public bool IsPayment { get; set; } = false;
        public int? ShopId { get; set; }
        public Shop? Shop { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
