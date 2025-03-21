using Core.DTOs.ShopDTO;
using Core.Entities.FollowEntities;
using Core.Entities.Identity;
using Core.Entities.PostEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.ShopEntities
{
    public class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? PictureUrl { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string? OwnerID { get; set; }
        public Coach? Owner { get; set; }
        public List<ShopPost>? Posts { get; set; }=new List<ShopPost>();
        public List<ShopFollow>? Followers { get; set; }=new List<ShopFollow>();
        public List<Product> Products { get; set; }
    }
}
