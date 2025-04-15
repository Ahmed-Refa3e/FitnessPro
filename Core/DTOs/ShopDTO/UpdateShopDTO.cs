using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO
{
    public class UpdateShopDTO
    {
        [Required]
        public string Name { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public required string City { get; set; }
        [Required]
        public required string Governorate { get; set; }
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
