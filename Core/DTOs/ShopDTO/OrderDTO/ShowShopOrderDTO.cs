namespace Core.DTOs.ShopDTO.OrderDTO
{
    public class ShowShopOrderDTO
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsPayment { get; set; }
        public bool IsRecieved { get; set; }
        public List<ShowOrderItemDTO>? OrderItems { get; set; }
    }
}
