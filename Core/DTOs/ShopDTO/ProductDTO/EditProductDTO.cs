using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO
{
    public class EditProductDTO
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
    }
}
