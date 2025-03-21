using Core.Entities.ShopEntities;

namespace Core.DTOs.ShopDTO
{
    public class ShowProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public ShowProductDTO()
        {
            
        }
        public ShowProductDTO(Product product)
        {
            this.Id = product.Id;
            this.Name = product.Name;
            this.Description = product.Description;
            this.Price = product.Price;
            this.ImagePath = product.ImagePath;
        }
    }
}
