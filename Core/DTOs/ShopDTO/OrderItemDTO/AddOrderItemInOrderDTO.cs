using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO
{
    public class AddOrderItemInOrderDTO
    {
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
