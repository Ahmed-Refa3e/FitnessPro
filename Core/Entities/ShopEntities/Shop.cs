using Core.DTOs.ShopDTO;
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
        public string? CoachID { get; set; }
        public Coach? Owner { get; set; }
        public List<ShopPost>? Posts { get; set; }
        public List<ShopFollow>? Followers { get; set; }
        public Shop()
        {
            
        }
        public Shop(AddShopDTO shop,string pictureUrl)
        {
            this.Name = shop.Name;
            this.PictureUrl = pictureUrl;
            this.Address = shop.Address;
            this.City = shop.City;
            this.Governorate = shop.Governorate;
            this.PhoneNumber = shop.PhoneNumber;
            this.Description = shop.Description;
            this.CoachID = shop.CoachID;
        }
        public void Update(UpdateShopDTO shop,string pictureUrl)
        {
            this.Name = shop.Name;
            this.Address= shop.Address;
            this.City= shop.City;
            this.Governorate= shop.Governorate;
            this.PhoneNumber = shop.PhoneNumber;
            this.Description= shop.Description;
            this.PictureUrl= pictureUrl;
        }
    }
}
