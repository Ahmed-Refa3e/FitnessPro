namespace Core.DTOs.ShopDTO
{
    public class ShowOrderItemInProductNeededListDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price at the time of purchase
    }
}
