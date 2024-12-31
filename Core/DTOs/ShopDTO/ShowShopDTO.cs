using Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ShopDTO
{
    public class ShowShopDTO
    {
        public string Name { get; set; }
        public string? PictureUrl { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string? OwnerName { get; set; }
    }
}
