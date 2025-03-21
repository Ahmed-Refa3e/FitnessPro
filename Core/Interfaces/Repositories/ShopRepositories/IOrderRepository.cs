using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IOrderRepository
    {
        ShowOrderDTO Get(int id);
        List<ShowOrderDTO> GetOrdersForUser(string userId);
        IntResult Add(AddOrderDTO order);
        IntResult MakeItReseved(int id);
        IntResult Delete(int id);
        IntResult IsReady(int id);
    }
}
