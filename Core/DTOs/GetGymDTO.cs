using System.ComponentModel.DataAnnotations;

namespace Core.DTOs
{
    public class GetGymDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 50, ErrorMessage = "Page size cannot be greater than 50.")]
        public int PageSize { get; set; } = 10;

        [MaxLength(100, ErrorMessage = "City name cannot exceed 100 characters.")]
        public string? City { get; set; }
    }
}
