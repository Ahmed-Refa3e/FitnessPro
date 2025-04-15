using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IOrderItemRepository
    {
        ShowOrderItemDTO GetById(int id);
        IntResult AddOrderItemInSpacificOrder(AddOrderItemDTO item);
        IntResult AddOrderItemInOrderDidnotReseived(AddOrderItemInOrderDTO item, string userId);
        IntResult UpdateOrderItem(EditOrderItemDTO item);
        IntResult DeleteOrderItem(int id);
        decimal GetPrice(int id);
    }
}
