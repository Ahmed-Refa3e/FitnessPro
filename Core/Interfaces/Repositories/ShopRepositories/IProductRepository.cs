using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.DTOs.ShopDTO.ProductDTO;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface IProductRepository
    {
        Task<List<ShowProductDTO>> GetProducts(ProductSearchDTO searchDTO);
        Task<ShowOneProductDTO> GetProductById(int productId);
        Task<IntResult> UpdateCategoriesOfProduct(ModifyCategoriesInProductDTO modifyCategories, string UserId);
        Task<IntResult> UpdateImage(UpdateImageDTO imageDTO, int productId, string userId);
        Task<IntResult> Add(AddProductDTO product, string userId);
        Task<IntResult> Delete(int productId, string userId);
        Task<IntResult> Update(EditProductDTO product, int id, string userId);
        Task<ItemPriceDTO> Decrease(int productId, int quantityNeeded);
    }
}
