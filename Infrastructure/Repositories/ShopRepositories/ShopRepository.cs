using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.ShopRepositories;
using Core.Interfaces.Services;
using Core.Utilities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories.IShopRepositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly FitnessContext _context;
        private readonly IBlobService _blobService;
        public ShopRepository(FitnessContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }
        public async Task<List<ShowShopDTO>> GetShopsWithFilter(SearchShopDTO searchDTO)
        {
            var shops = _context.Shops.AsQueryable();
            if (!string.IsNullOrEmpty(searchDTO.Governorate))
            {
                shops=shops.Where(x=>x.Governorate== searchDTO.Governorate);
            }
            if (!string.IsNullOrEmpty(searchDTO.City))
            {
                shops = shops.Where(x => x.City == searchDTO.City);
            }
            if (searchDTO.OrderedByTheMostFollowerNumber)
            {
                shops = shops.OrderByDescending(x => x.Followers.Count());
            }
            (searchDTO.PageNumber, searchDTO.PageSize) = await PaginationHelper.NormalizePaginationAsync(
                shops,
                searchDTO.PageNumber,
                searchDTO.PageSize
            );
            return await shops
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
                .Select(s => new ShowShopDTO
                {
                    ShopId = s.Id,
                    Address = s.Address,
                    City = s.City,
                    OwnerName = $"{s.Owner.FirstName} {s.Owner.LastName}" ?? "",
                    Description = s.Description,
                    Governorate = s.Governorate,
                    ShopName = s.Name,
                    PhoneNumber = s.PhoneNumber,
                    PictureUrl = s.PictureUrl,
                    OwnerID = s.OwnerID,
                    FollowerNumber = _context.ShopFollows.Count(f => f.ShopId == s.Id)
                })
                .ToListAsync();
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
                Name = shopDto.ShopName,
                Address = shopDto.Address,
                City = shopDto.City,
                Governorate = shopDto.Governorate,
                PhoneNumber = shopDto.PhoneNumber,
                Description = shopDto.Description,
                OwnerID = userId
            };
            await _context.Shops.AddAsync(newShop);
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.SaveChangesAsync();
                if(shopDto.Image is not null)
                {
                    var result = AddImageHelper.CheckImage(shopDto.Image);
                    if(result.Id==0) {
                        return result; 
                    }
                    newShop.PictureUrl=await _blobService.UploadImageAsync(shopDto.Image);
                    await _context.SaveChangesAsync();
                }
                await transaction.CommitAsync();
                return new IntResult { Id = newShop.Id };
            }
            catch (Exception ex)
            {
                await _blobService.DeleteImageAsync(newShop.PictureUrl);
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
                await _blobService.DeleteImageAsync(shop.PictureUrl);
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
                    ShopId = s.Id,
                    Address = s.Address,
                    City = s.City,
                    OwnerName = $"{s.Owner.FirstName} {s.Owner.LastName}" ?? "",
                    Description = s.Description,
                    Governorate = s.Governorate,
                    ShopName = s.Name,
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
                    ShopId = s.Id,
                    Address = s.Address,
                    City = s.City,
                    OwnerName = $"{s.Owner.FirstName} {s.Owner.LastName}" ?? "",
                    Description = s.Description,
                    Governorate = s.Governorate,
                    ShopName = s.Name,
                    PhoneNumber = s.PhoneNumber,
                    PictureUrl = s.PictureUrl,
                    OwnerID = s.OwnerID,
                    FollowerNumber = _context.ShopFollows.Count(f => f.ShopId == s.Id)
                }).ToListAsync();
        }
        public async Task<IntResult> Update(UpdateShopDTO shopDto,int shopId, string userId)
        {
            var existingShop = await _context.Shops.FindAsync(shopId);
            if (existingShop is null||existingShop.OwnerID!=userId)
            {
                return new IntResult { Massage = "You did not have a shop." };
            }
            existingShop.Name = shopDto.ShopName;
            existingShop.Address = shopDto.Address;
            existingShop.City = shopDto.City;
            existingShop.Governorate = shopDto.Governorate;
            existingShop.PhoneNumber = shopDto.PhoneNumber;
            existingShop.Description = shopDto.Description;
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
        public async Task<IntResult> UpdateImage(UpdateImageDTO imageDTO, int shopId, string userId)
        {
            var existingShop = await _context.Shops.FindAsync(shopId);
            if (existingShop is null || existingShop.OwnerID != userId)
            {
                return new IntResult { Massage = "You did not have a shop." };
            }
            var oldPath = existingShop.PictureUrl;
            if (imageDTO.Image is null)
            {
                existingShop.PictureUrl = null;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    return new IntResult { Massage=ex.Message};
                }
                await _blobService.DeleteImageAsync(oldPath);
            }
            else
            {
                var result = AddImageHelper.CheckImage(imageDTO.Image);
                if (result.Id == 0)
                {
                    return result;
                }
                existingShop.PictureUrl = await _blobService.UploadImageAsync(imageDTO.Image);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    await _blobService.DeleteImageAsync(existingShop.PictureUrl);
                    return new IntResult { Massage = ex.Message };
                }
                await _blobService.DeleteImageAsync(oldPath);
            }
            return new IntResult { Id = shopId };
        }
    }
}
