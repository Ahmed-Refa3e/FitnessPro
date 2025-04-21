using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.DTOs.ShopDTO.OrderDTO;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IOrderRepository
    {
        ShowOrderDTO GetOrder(int id, string userId);
        List<ShowUserOrderDTO> GetOrdersForUser(string userId);
        List<ShowShopOrderDTO> GetOrdersForShop(int shopId, string userId);
        IntResult Add(AddOrderDTO order, string userId);
        IntResult MakeItReseved(int id, string userId);
        IntResult MakeItPaymented(int id, string userId);
        IntResult Delete(int id,string userId);
    }
}
