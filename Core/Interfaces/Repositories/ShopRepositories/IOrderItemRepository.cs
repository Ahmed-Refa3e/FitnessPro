using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IOrderItemRepository
    {
        ShowOrderItemDTO GetById(int id);
        IntResult AddOrderItemInSpacificOrder(AddOrderItemDTO item);
        IntResult UpdateOrderItem(EditOrderItemDTO item, int id);
        IntResult DeleteOrderItem(int id);
        decimal GetPrice(int id);
        IntResult AddOrderItemInOrderDidnotReseived(AddOrderItemInOrderDTO item, string userId);
        IntResult MakeItReady(int id);
    }
}
