using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.DTOs.ShopDTO.OrderDTO;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IOrderRepository
    {
        ShowOrderDTO GetOrder(int id);
        List<ShowUserOrderDTO> GetOrdersForUser(string userId);
        List<ShowShopOrderDTO> GetOrdersForShop(int shopId);
        IntResult Add(AddOrderDTO order);
        IntResult MakeItReseved(int id);
        IntResult Delete(int id);
    }
}
