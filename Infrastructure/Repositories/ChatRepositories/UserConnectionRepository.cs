using Core.Entities.ChatEntites;
using Core.Interfaces.Repositories.ChatRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.ChatRepositories
{
    public class UserConnectionRepository : GenericRepository<UserConnection>, IUserConnectionRepository
    {
        private readonly FitnessContext context;

        public UserConnectionRepository(FitnessContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> IsUserOnlineAsync(string userId)
        {
            return await context.connections.AnyAsync(c => c.userId == userId);
        }

        public async Task<List<string>> GetConnectedUserIdsAsync()
        {
            return await context.connections
                .Select(c => c.userId)
                .Distinct()
                .ToListAsync();
        }

        public async Task RemoveConnectionAsync(string userId, string connectionId)
        {
            var connection = await context.connections
                .FirstOrDefaultAsync(c => c.userId == userId && c.connectionId == connectionId);

            if (connection != null)
            {
                context.connections.Remove(connection);
                await context.SaveChangesAsync();
            }
        }
    }
}
