using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.PostDTO
{
    public class AddShopPostDTO : AddPostDTO
    {
        [Required]
        public int ShopId { get; set; }
    }
}
