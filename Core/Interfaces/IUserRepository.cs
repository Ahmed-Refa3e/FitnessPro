using Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
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
