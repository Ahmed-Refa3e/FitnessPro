namespace Infrastructure.SeedData.SeedingHelpers
{
    public class SeedingGym
    {
        public required string GymName { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string Governorate { get; set; }
        public int MonthlyPrice { get; set; }
        public int? YearlyPrice { get; set; }
        public int? FortnightlyPrice { get; set; }
        public int? SessionPrice { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string? CoachEmail { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
