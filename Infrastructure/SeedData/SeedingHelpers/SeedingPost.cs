namespace Infrastructure.SeedData.SeedingHelpers
{
    public class SeedingPost
    {
        public required string Type { get; set; } // COACH, GYM, SHOP
        public required string EmailOrName { get; set; }
        public required string Content { get; set; }
        public List<string>? PictureUrls { get; set; }
    }
}
