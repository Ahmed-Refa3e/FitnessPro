using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.GymDTO
{
    public class GetNearbyGymsDTO
    {
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}
