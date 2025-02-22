using Core.Entities.FollowEntities;
using Core.Entities.Identity;
using System.Linq.Expressions;

namespace Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        IQueryable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>>? expression = null, string? includeProperties = null);
        Task<ApplicationUser?> GetAsync(Expression<Func<ApplicationUser, bool>> expression, string? includeProperties = null);
        Task<ApplicationUser?> GetByIdAsync(int id);

        Task AddAsync(ApplicationUser entity);

        Task DeleteAsync(ApplicationUser entity);
        Task RemoveRangeAsync(List<ApplicationUser> entities);
        Task SaveAsync();
        
        Task AddFollower(UserFollow follow);
        Task<UserFollow> GetFollow(string userId, string FollowedId);
        void RemoveFollow(UserFollow follow);

        Task AddGymFollower(GymFollow gymFollow);
        Task<GymFollow> GetGymFollow(string userId, int gymId);
        void RemoveGymFollow(GymFollow follow);

        Task AddShopFollower(ShopFollow shopFollow);
        Task<ShopFollow> GetShopFollow(string userId, int shopId);
        void RemoveShopFollow(ShopFollow follow);

    }
}
