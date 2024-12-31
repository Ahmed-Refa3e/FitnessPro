using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IShopRepository
    {
        Task<IntResult> Add(AddShopDTO shop);
        Task<IntResult> Update(UpdateShopDTO shop,int id);
        IntResult Delete(int id);
        ShowShopDTO GetShop(int id);
    }
}
