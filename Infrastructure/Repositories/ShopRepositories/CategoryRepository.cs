using Core.DTOs.ShopDTO;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.ShopRepositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories.ShopRepositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FitnessContext _context;
        public CategoryRepository(FitnessContext context)
        {
            _context = context;
        }
        public Category CheckIfItexistingAndGet(string categoryName)
        {
            var category = _context.categories.FirstOrDefault(x => x.Name == categoryName);
            if (category is null)
            {
                category = new Category { Name = categoryName };
                _context.categories.Add(category);
                _context.SaveChanges();
            }
            return category;
        }
        public List<ShowCategoryDTO> GetAll()
        {
            var result = new List<ShowCategoryDTO>();
            var categories = _context.categories.ToList();
            foreach (var category in categories)
            {
                result.Add(new ShowCategoryDTO { Id = category.Id, Name = category.Name });
            }
            return result;
        }
    }
}
