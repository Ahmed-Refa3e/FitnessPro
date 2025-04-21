using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO.ProductDTO
{
    public class ModifyCategoriesInProductDTO
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(10)]
        public List<string> CategoriesName { get; set; }
    }
}
