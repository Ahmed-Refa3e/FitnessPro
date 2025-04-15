using Core.Entities.ChatEntites;

namespace Core.Interfaces.Repositories.ChatRepositories
{
    public interface IUserConnectionRepository : IGenericRepository<UserConnection>
    {
        Task<bool> IsUserOnlineAsync(string userId);
        Task<List<string>> GetConnectedUserIdsAsync();
        Task RemoveConnectionAsync(string userId, string connectionId);
    }
}
