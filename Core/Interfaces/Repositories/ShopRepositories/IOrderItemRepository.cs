using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IOrderItemRepository
    {
        Task<ShowOrderItemDTO> GetById(int id, string userId);
        Task<IntResult> AddOrderItemInSpacificOrder(AddOrderItemDTO item);
        Task<IntResult> AddOrderItemInSpacificOrderWithCheckUserID(AddOrderItemDTO item, string userId);
        Task<IntResult> AddOrderItemInOrderDidnotReseived(AddOrderItemInOrderDTO item, string userId);
        Task<IntResult> UpdateOrderItem(EditOrderItemDTO item, string userId);
        Task<IntResult> DeleteOrderItem(int id);
        Task<IntResult> DeleteOrderItemWithUserId(int id, string userId);
    }
}
