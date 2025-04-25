using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.DTOs.ShopDTO.OrderDTO;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IOrderRepository
    {
        Task<ShowOrderDTO> GetOrder(int id, string userId);
        Task<List<ShowUserOrderDTO>> GetOrdersForUser(string userId);
        Task<List<ShowShopOrderDTO>> GetOrdersForShop(int shopId, string userId);
        Task<IntResult> Add(AddOrderDTO order, string userId);
        Task<IntResult> MakeItReseved(int id, string userId);
        Task<IntResult> MakeItPaymented(int id, string userId);
        Task<IntResult> Delete(int id,string userId);
    }
}
