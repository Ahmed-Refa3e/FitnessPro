namespace Core.DTOs.ShopDTO
{
    public class ProductOrderedFromSellerDTO
    {
        public string ProductName { get; set; }
        public int NumberOfProductNeeded { get; set; }
        public List<ShowOrderItemInProductNeededListDTO> OrderItemsNeeded { get; set; }
    }
}
