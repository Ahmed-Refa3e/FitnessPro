namespace Infrastructure.SeedData.SeedingHelpers
{
    public class SeedingProduct
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public decimal OfferPrice { get; set; }
        public string? ImagePath { get; set; }
        public int Quantity { get; set; }
        public List<string> Categories { get; set; } = new();
        public required string ShopName { get; set; }
    }
}
