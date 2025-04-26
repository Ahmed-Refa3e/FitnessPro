using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IShopRepository
    {
        Task<IntResult> Add(AddShopDTO shop, string userId);
        Task<IntResult> Update(AddShopDTO  shop,int shopId,string userId);
        Task<IntResult> Delete(string userId,int shopId);
        Task<ShowShopDTO> GetShop(int id);
        Task<List<ShowShopDTO>> GetShopsWithFilter(SearchShopDTO searchDTO);
        Task<List<ShowShopDTO>> GetShopsOfOwner(string userId);
    }
}
