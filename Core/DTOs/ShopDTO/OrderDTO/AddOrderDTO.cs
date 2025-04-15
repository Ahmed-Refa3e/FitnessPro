using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO
{
    public class AddOrderDTO
    {
        public List<AddOrderItemInOrderDTO>? OrderItems { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
