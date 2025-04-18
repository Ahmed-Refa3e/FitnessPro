using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IOrderItemRepository
    {
        ShowOrderItemDTO GetById(int id, string userId);
        IntResult AddOrderItemInSpacificOrder(AddOrderItemDTO item);
        public IntResult AddOrderItemInSpacificOrderWithCheckUserID(AddOrderItemDTO item, string userId);
        IntResult AddOrderItemInOrderDidnotReseived(AddOrderItemInOrderDTO item, string userId);
        IntResult UpdateOrderItem(EditOrderItemDTO item, string userId);
        IntResult DeleteOrderItem(int id);
        IntResult DeleteOrderItemWithUserId(int id, string userId);
        decimal GetPrice(int id);
    }
}
