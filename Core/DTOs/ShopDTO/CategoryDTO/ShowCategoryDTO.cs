using Core.Entities.ShopEntities;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO
{
    public class ShowCategoryDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public ShowCategoryDTO()
        {
            
        }
        public ShowCategoryDTO(Category category)
        {
            this.Name = category.Name;
            this.Description = category.Description;
        }
    }
}
