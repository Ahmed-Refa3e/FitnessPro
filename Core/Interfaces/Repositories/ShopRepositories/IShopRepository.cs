using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IShopRepository
    {
        Task<IntResult> Add(AddShopDTO shop, string userId);
        Task<IntResult> Update(AddShopDTO  shop,int id, string userId);
        Task<IntResult> DeleteAsync(int id,string userId);
        Task<ShowShopDTO> GetShop(int id);
    }
}
