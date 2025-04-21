using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO
{
    public class AddOrderItemInOrderDTO
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
