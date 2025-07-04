﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.ShopDTO
{
    public class AddShopDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        public string ShopName { get; set; }
        //public IFormFile? Image {  get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Governorate is required.")]
        public string Governorate { get; set; }

        [MaxLength(15, ErrorMessage = "Phone number must not exceed 15 digits.")]
        public string? PhoneNumber { get; set; }

        [MaxLength(500, ErrorMessage = "Description must not exceed 500 characters.")]
        public string? Description { get; set; }
    }

}
