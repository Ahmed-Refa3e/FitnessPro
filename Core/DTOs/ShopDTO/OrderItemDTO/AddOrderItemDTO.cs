using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO
{
    public class AddOrderItemDTO
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
