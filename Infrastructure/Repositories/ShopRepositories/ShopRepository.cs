using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.ShopRepositories;
using Core.Utilities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.IShopRepositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImagesForShop");
        private readonly FitnessContext _context;
        public ShopRepository(FitnessContext context)
        {
            this._context = context;
        }
        public async Task<IntResult> Add(AddShopDTO shop)
        {
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
            if (_context.applicationUsers.Find(shop.CoachID) is null)
            {
                return new IntResult() { Massage = "Id not valid" };
            }
            var filePath = AddImageHelper.chickImagePath(shop.Image, _storagePath);
            if (!string.IsNullOrEmpty(filePath.Massage))
            {
                return new IntResult() { Massage = filePath.Massage };
            }
            var newShop = new Shop();
            newShop.Name = shop.Name;
            newShop.PictureUrl = filePath.Id;
            newShop.Address = shop.Address;
            newShop.City = shop.City;
            newShop.Governorate = shop.Governorate;
            newShop.PhoneNumber = shop.PhoneNumber;
            newShop.Description = shop.Description;
            newShop.OwnerID = shop.CoachID;
            _context.Shops.Add(newShop);
            try
            {
                await _context.SaveChangesAsync();
                if (!string.IsNullOrEmpty(filePath.Id))
                {
                    using (var stream = new FileStream(filePath.Id, FileMode.Create))
                    {
                        await shop.Image.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                return new IntResult() { Massage = ex.Message };
            }
            return new IntResult() { Id = newShop.Id };
        }
        public IntResult Delete(int id)
        {
            var shop = Get(id);
            if(shop is null) {
                return new IntResult() { Massage = "Id is not valid" };
            }
            _context.Shops.Remove(shop);
            try
            {
                _context.SaveChanges();
                if (File.Exists(shop.PictureUrl))
                {
                    File.Delete(shop.PictureUrl);
                }
            }
            catch (Exception ex) { 
                return new IntResult() { Massage = ex.Message }; 
            }
            return new IntResult() { Id = 1 };
        }
        public ShowShopDTO GetShop(int id)
        {
            return _context.Shops.Include(e=>e.Followers).Where(x => x.Id == id).Select(x => new ShowShopDTO
            {
                Address = x.Address,
                City = x.City,
                OwnerName = (x.Owner.FirstName + " " + x.Owner.LastName)??"",
                Description = x.Description,
                Governorate = x.Governorate,
                Name = x.Name,
                PhoneNumber = x.PhoneNumber,
                PictureUrl = x.PictureUrl
            }).FirstOrDefault();
        }
        public async Task<IntResult> Update(UpdateShopDTO shop, int id)
        {
            var oldShop = Get(id);
            if (oldShop is null)
            {
                return new IntResult { Massage = "Not valid Id" };
            }
            var filePath = AddImageHelper.chickImagePath(shop.Image, _storagePath);
            if (!string.IsNullOrEmpty(filePath.Massage))
            {
                return new IntResult() { Massage = filePath.Massage };
            }
            var oldPath = oldShop.PictureUrl;
            oldShop.Name = shop.Name;
            oldShop.Address = shop.Address;
            oldShop.City = shop.City;
            oldShop.Governorate = shop.Governorate;
            oldShop.PhoneNumber = shop.PhoneNumber;
            oldShop.Description = shop.Description;
            oldShop.PictureUrl = filePath.Id;
            try
            {
                await _context.SaveChangesAsync();
                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }
                if (!string.IsNullOrEmpty(filePath.Id))
                {
                    using (var stream = new FileStream(filePath.Id, FileMode.Create))
                    {
                        await shop.Image.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                return new IntResult() { Massage = ex.Message };
            }
            return new IntResult { Id = oldShop.Id };
        }
        Shop Get(int id) => _context.Shops.Find(id);
    }
}
