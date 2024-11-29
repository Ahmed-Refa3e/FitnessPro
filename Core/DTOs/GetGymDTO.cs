using System.ComponentModel.DataAnnotations;

namespace Core.DTOs
{
    public class GetGymDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 50, ErrorMessage = "Page size must be between 1 and 50.")]
        public int PageSize { get; set; } = 10;

        [MaxLength(100, ErrorMessage = "City name cannot exceed 100 characters.")]
        public string? City { get; set; }

        [MaxLength(100, ErrorMessage = "Governorate name cannot exceed 100 characters.")]
        public string? Governorate { get; set; }

        [MaxLength(100, ErrorMessage = "Gym name cannot exceed 100 characters.")]
        public string? GymName { get; set; }

        [RegularExpression("subscriptions|rating|highestPrice|lowestPrice", ErrorMessage = "SortBy must be either 'subscriptions', 'rating', 'highestPrice', or 'lowestPrice'.")]
        public string? SortBy { get; set; }
    }
}
