using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO
{
    public class AddOrderDTO
    {
        [Required]
        public List<AddOrderItemInOrderDTO>? OrderItems { get; set; }
    }
}
