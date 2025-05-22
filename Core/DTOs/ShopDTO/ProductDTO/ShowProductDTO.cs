namespace Core.DTOs.ShopDTO
{
    public class ShowProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal OfferPrice {  get; set; }
        public decimal Discount {  get; set; }
        public string ImagePath { get; set; }
    }
}
