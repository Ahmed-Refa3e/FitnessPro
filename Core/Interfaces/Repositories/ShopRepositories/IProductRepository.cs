using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.DTOs.ShopDTO.ProductDTO;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IProductRepository
    {
        List<ShowProductDTO> GetProductsInPaginationsNoCategory(int page, int pageSize);
        List<ShowProductDTO> GetProductsInPaginationsOnShopNoCategory(int shopId, int page, int pageSize);
        List<ShowProductDTO> GetProductsInPaginationsOnShopWithCategory(int shopID, int page, int pageSize, int categoryId);
        List<ShowProductDTO> GetProductsInPaginationsWithCategory(int page, int pageSize, int categoryId);
        ShowOneProductDTO GetProductById(int productId);
        IntResult UpdateCategoriesOfProduct(ModifyCategoriesInProductDTO modifyCategories, string UserId);
        Task<IntResult> Add(AddProductDTO product, string userId);
        IntResult Delete(int productId, string userId);
        Task<IntResult> Update(EditProductDTO product, int id, string userId);
        ItemPriceDTO Decrease(int productId, int quantityNeeded);
        int ShowProductShopId(int productId);
    }
}
