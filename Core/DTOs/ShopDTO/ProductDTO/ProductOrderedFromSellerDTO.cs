using Core.Entities.ShopEntities;

namespace Core.DTOs.ShopDTO
{
    public class ProductOrderedFromSellerDTO
    {
        public string ProductName { get; set; }
        public int NumberOfProductNeeded { get; set; }
        public List<ShowOrderItemInProductNeededListDTO> OrderItemsNeeded { get; set; }
        public ProductOrderedFromSellerDTO(Product product)
        {
            ProductName = product.Name;
            NumberOfProductNeeded = 0;
            OrderItemsNeeded = new List<ShowOrderItemInProductNeededListDTO>();
            foreach (var item in product.OrderItems)
            {
                NumberOfProductNeeded += item.Quantity;
                OrderItemsNeeded.Add(new ShowOrderItemInProductNeededListDTO() { Id = item.Id, Price = item.Price, Quantity = item.Quantity });
            }
        }
    }
}
