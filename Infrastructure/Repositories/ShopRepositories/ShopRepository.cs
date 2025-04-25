using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.ShopRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.IShopRepositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly FitnessContext _context;
        public ShopRepository(FitnessContext context)
        {
            _context = context;
        }
        public async Task<IntResult> Add(AddShopDTO shopDto, string userId)
        {
            var user = await _context.applicationUsers.FindAsync(userId);
            if (user is null)
            {
                return new IntResult { Massage = "Id not valid" };
            }
            var newShop = new Shop
            {
                Name = shopDto.Name,
                PictureUrl = shopDto.ImageUrl,
                Address = shopDto.Address,
                City = shopDto.City,
                Governorate = shopDto.Governorate,
                PhoneNumber = shopDto.PhoneNumber,
                Description = shopDto.Description,
                OwnerID = userId
            };
            _context.Shops.Add(newShop);
            try
            {
                await _context.SaveChangesAsync();
                return new IntResult { Id = newShop.Id };
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
        }

        public async Task<IntResult> Delete(string userId, int shopId)
        {
            var shop = await _context.Shops.FindAsync(shopId);
            if (shop is null||shop.OwnerID!=userId)
            {
                return new IntResult { Massage = "You did not have a shop." };
            }
            _context.Shops.Remove(shop);
            try
            {
                await _context.SaveChangesAsync();
                return new IntResult { Id = 1 };
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
        }
        public async Task<ShowShopDTO> GetShop(int id)
        {
            return await _context.Shops.Where(s => s.Id == id)
                .Select(s => new ShowShopDTO
                {
                    GymId = s.Id,
                    Address = s.Address,
                    City = s.City,
                    OwnerName = $"{s.Owner.FirstName} {s.Owner.LastName}" ?? "",
                    Description = s.Description,
                    Governorate = s.Governorate,
                    GymName = s.Name,
                    PhoneNumber = s.PhoneNumber,
                    PictureUrl = s.PictureUrl,
                    OwnerID = s.OwnerID,
                    FollowerNumber = _context.ShopFollows.Count(f => f.ShopId == s.Id)
                })
                .FirstOrDefaultAsync();
        }
        public async Task<List<ShowShopDTO>> GetShopsOfOwner(string userId)
        {
            return await _context.Shops.Where(s => s.OwnerID == userId)
                .Select(s => new ShowShopDTO
                {
                    GymId = s.Id,
                    Address = s.Address,
                    City = s.City,
                    OwnerName = $"{s.Owner.FirstName} {s.Owner.LastName}" ?? "",
                    Description = s.Description,
                    Governorate = s.Governorate,
                    GymName = s.Name,
                    PhoneNumber = s.PhoneNumber,
                    PictureUrl = s.PictureUrl,
                    OwnerID = s.OwnerID,
                    FollowerNumber = _context.ShopFollows.Count(f => f.ShopId == s.Id)
                }).ToListAsync();
        }
        public async Task<IntResult> Update(AddShopDTO shopDto,int shopId, string userId)
        {
            var existingShop = await _context.Shops.FindAsync(shopId);
            if (existingShop is null||existingShop.OwnerID!=userId)
            {
                return new IntResult { Massage = "You did not have a shop." };
            }
            existingShop.Name = shopDto.Name;
            existingShop.Address = shopDto.Address;
            existingShop.City = shopDto.City;
            existingShop.Governorate = shopDto.Governorate;
            existingShop.PhoneNumber = shopDto.PhoneNumber;
            existingShop.Description = shopDto.Description;
            existingShop.PictureUrl = shopDto.ImageUrl;
            try
            {
                await _context.SaveChangesAsync();
                return new IntResult { Id = existingShop.Id };
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
        }
    }
}
