using Core.DTOs.ShopDTO;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.ShopRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.ShopRepositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FitnessContext _context;
        public CategoryRepository(FitnessContext context)
        {
            _context = context;
        }
        public async Task<Category> CheckIfItexistingAndGet(string categoryName)
        {
            var category = await _context.categories.FirstOrDefaultAsync(x => x.Name == categoryName);

            if (category is null)
            {
                category = new Category { Name = categoryName };
                await _context.categories.AddAsync(category);
                await _context.SaveChangesAsync();
            }

            return category;
        }
        public async Task<List<ShowCategoryDTO>> GetAll()
        {
            return await _context.categories.Select(category => new ShowCategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            }).ToListAsync();
        }
    }
}
