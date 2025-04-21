namespace Core.DTOs.ShopDTO.OrderDTO
{
    public class ShowUserOrderDTO
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string? ShopName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsPayment { get; set; }
        public bool IsRecieved { get; set; }
        public List<ShowOrderItemDTO>? OrderItems { get; set; }
    }
}
