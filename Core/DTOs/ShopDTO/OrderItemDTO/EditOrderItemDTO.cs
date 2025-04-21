using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO
{
    public class EditOrderItemDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
