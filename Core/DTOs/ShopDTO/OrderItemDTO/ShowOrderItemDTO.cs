namespace Core.DTOs.ShopDTO
{
    public class ShowOrderItemDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public bool IsReady { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
    }
}
