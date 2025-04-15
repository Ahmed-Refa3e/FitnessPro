namespace Core.DTOs.ShopDTO
{
    public class ShowOneProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public List<string> CategoriesName { get; set; }
        public string ShopName { get; set; }
        public string ShopImage { get; set; }
    }
}
