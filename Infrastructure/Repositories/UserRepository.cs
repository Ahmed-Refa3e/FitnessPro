using Core.Entities.FollowEntities;
using Core.Entities.GymEntities;
using Core.Entities.Identity;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FitnessContext context;

        public UserRepository(FitnessContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(ApplicationUser entity)
        {
            await context.AddAsync(entity);
        }

        public async Task DeleteAsync(ApplicationUser entity)
        {
            context.Remove(entity);
            await SaveAsync();
        }

        public async Task<ApplicationUser?> GetAsync(Expression<Func<ApplicationUser, bool>> expression, string? includeProperties = null)
        {
            IQueryable<ApplicationUser> query = context.Set<ApplicationUser>();
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return await query.FirstOrDefaultAsync(expression);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync(Expression<Func<ApplicationUser, bool>>? expression = null, string? includeProperties = null)
        {
            IQueryable<ApplicationUser> query = context.Set<ApplicationUser>();
            if (expression != null)
            {
                query = query.Where(expression);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<ApplicationUser?> GetByIdAsync(int id)
        {
            return await context.Set<ApplicationUser>().FindAsync(id);
        }

        public async Task RemoveRangeAsync(List<ApplicationUser> entities)
        {
            context.Set<ApplicationUser>().RemoveRange(entities);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task AddFollower(UserFollow follow)
        {
            await context.userFollows.AddAsync(follow);
        }

        public async Task<UserFollow> GetFollow(string userId, string FollowedUd)
        {
            var Follow= await context.userFollows
                 .FirstOrDefaultAsync(e => e.FollowingId == FollowedUd && e.FollowerId == userId);

                return Follow;
        }

        public void RemoveFollow(UserFollow follow)
        {
            context.userFollows.Remove(follow);
        }

        public async Task AddGymFollower(GymFollow follow)
        {
            await context.gymFollows.AddAsync(follow);
        }

        public async Task<GymFollow> GetGymFollow(string userId, int gymId)
        {
            var Follow = await context.gymFollows
                 .FirstOrDefaultAsync(e => e.FollowerId == userId && e.GymId == gymId);

            return Follow;
        }

        public void RemoveGymFollow(GymFollow follow)
        {
            context.gymFollows.Remove(follow);
        }

        public async Task AddShopFollower(ShopFollow shopFollow)
        {
            await context.ShopFollows.AddAsync(shopFollow);
        }

        public async Task<ShopFollow> GetShopFollow(string userId, int shopId)
        {
            var Follow = await context.ShopFollows
                 .FirstOrDefaultAsync(e => e.FollowerId == userId && e.ShopId == shopId);

            return Follow;
        }

        public void RemoveShopFollow(ShopFollow follow)
        {
            context.ShopFollows.Remove(follow);
        }
    }
}
