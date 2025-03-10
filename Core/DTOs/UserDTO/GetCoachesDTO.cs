using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.UserDTO
{
    public class GetCoachesDTO
    {
        [Range(1, 50, ErrorMessage = "Page size must be between 1 and 50.")]
        public int PageSize { get; set; } = 10;
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1.")]
        public int PageNumber { get; set; } = 1;
        [MaxLength(50, ErrorMessage = "Coach name cannot exceed 50 characters.")]
        public string? CoachName { get; set; }
        [Range(0, 5, ErrorMessage = "Min rating must be between 0 and 5.")]
        public double? MinRating { get; set; }

        [Range(0, 5, ErrorMessage = "Max rating must be between 0 and 5.")]
        public double? MaxRating { get; set; }
        [RegularExpression("firstName|rating|joinedDate", ErrorMessage = "SortBy must be 'firstName', 'rating', or 'joinedDate'.")]
        public string? SortBy { get; set; }
    }
}
