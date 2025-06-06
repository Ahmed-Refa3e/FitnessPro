﻿namespace Core.DTOs.GymDTO
{
    public class GymResponseDto
    {
        public int GymID { get; set; }
        public string? GymName { get; set; }
        public string? PictureUrl { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public int? MonthlyPrice { get; set; }
        public decimal AverageRating { get; set; }
        public int SubscriptionsCount { get; set; }
    }
}
