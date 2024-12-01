using Core.Entities.Identity;
using System.Linq.Expressions;

namespace Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync(Expression<Func<ApplicationUser, bool>>? expression = null, string? includeProperties = null);
        Task<ApplicationUser?> GetAsync(Expression<Func<ApplicationUser, bool>> expression, string? includeProperties = null);
        Task<ApplicationUser?> GetByIdAsync(int id);

        Task AddAsync(ApplicationUser entity);

        Task DeleteAsync(ApplicationUser entity);
        Task RemoveRangeAsync(List<ApplicationUser> entities);
        Task SaveAsync();
    }
}
