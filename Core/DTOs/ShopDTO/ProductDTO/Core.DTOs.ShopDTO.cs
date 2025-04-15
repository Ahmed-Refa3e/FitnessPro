using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO
{
    public class AddProductDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public IFormFile Image { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [MaxLength(10)]
        [MinLength(1)]
        public List<string> CategoriesName { get; set; }
        [Required]
        public int ShopId { get; set; }
    }
}
