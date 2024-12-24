using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.GymDTO
{
    public class UpdateGymDTO
    {
        [Required(ErrorMessage = "Gym name is required.")]
        [MaxLength(100, ErrorMessage = "Gym name cannot exceed 100 characters.")]
        public required string GymName { get; set; }

       //[Url(ErrorMessage = "Picture URL must be a valid URL.")]
        public string? PictureUrl { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(50, ErrorMessage = "City name cannot exceed 50 characters.")]
        public required string City { get; set; }

        [Required(ErrorMessage = "Governorate is required.")]
        [MaxLength(50, ErrorMessage = "Governorate name cannot exceed 50 characters.")]
        public required string Governorate { get; set; }

        [Required(ErrorMessage = "Monthly price is required.")]
        [Range(1, 10000, ErrorMessage = "Monthly price must be between 1 and 10,000.")]
        public required int MonthlyPrice { get; set; }

        [Range(1, 100000, ErrorMessage = "Yearly price must be between 1 and 100,000.")]
        public int? YearlyPrice { get; set; }

        [Range(1, 5000, ErrorMessage = "Fortnightly price must be between 1 and 5,000.")]
        public int? FortnightlyPrice { get; set; }

        [Range(1, 1000, ErrorMessage = "Session price must be between 1 and 1,000.")]
        public int? SessionPrice { get; set; }

        [Phone(ErrorMessage = "Phone number is not valid.")]
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string? PhoneNumber { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
