using Core.DTOs.ShopDTO;
using Core.Entities.ShopEntities;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface ICategoryRepository
    {
        List<ShowCategoryDTO> GetAll();
        Category CheckIfItexistingAndGet(string categoryName);
    }
}
