using Core.DTOs.ShopDTO;
using Core.Entities.ShopEntities;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface ICategoryRepository
    {
        Task<List<ShowCategoryDTO>> GetAll();
        Task<Category> CheckIfItexistingAndGet(string categoryName);
    }
}
