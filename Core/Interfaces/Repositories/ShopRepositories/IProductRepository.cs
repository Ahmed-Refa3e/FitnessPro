using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.Entities.ShopEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IProductRepository
    {
        List<ShowProductDTO> GetProductsInPaginationsNoCategory(int page, int pageSize);
        List<ShowProductDTO> GetProductsInPaginationsWithCategory(int page, int pageSize, int categoryId);
        ShowOneProductDTO GetProductById(int productId);
        Task<IntResult> Add(AddProductDTO product);
        IntResult Delete(int productId);
        Task<IntResult> Update(EditProductDTO product, int id);
        ItemPriceDTO Decrease(int productId, int quantityNeeded);
        Product GetProductWithItems(int productId);
        string ShowProductSeller(int productId);
    }
}
