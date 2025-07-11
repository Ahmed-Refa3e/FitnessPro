namespace Infrastructure.SeedData.SeedingHelpers
{
    public class SeedingShop
    {
        public required string ShopName { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string Governorate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string? CoachEmail { get; set; }
        public string? PictureUrl { get; set; }
    }
}
