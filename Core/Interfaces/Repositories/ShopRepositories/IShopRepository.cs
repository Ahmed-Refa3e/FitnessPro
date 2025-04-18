using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IShopRepository
    {
        Task<IntResult> Add(AddShopDTO shop, string userId);
        Task<IntResult> Update(UpdateShopDTO shop, string userId);
        IntResult Delete(int id,string userId);
        ShowShopDTO GetShop(int id);
    }
}
