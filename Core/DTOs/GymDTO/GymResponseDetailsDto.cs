namespace Core.DTOs.GymDTO
{
    public class GymResponseDetailsDto
    {
        public int GymID { get; set; }
        public string? GymName { get; set; }
        public string? PictureUrl { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public int? MonthlyPrice { get; set; }
        public int? YearlyPrice { get; set; }
        public int? FortnightlyPrice { get; set; }
        public int? SessionPrice { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string? CoachID { get; set; }
        public string? CoachFullName { get; set; }
        public string? CoachProfilePictureUrl { get; set; }
        public decimal AverageRating { get; set; }
        public int SubscriptionsCount { get; set; }
    }
}
