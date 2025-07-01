using Core.Interfaces.Repositories.OnlineTrainingRepositories;

namespace Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGymRepository GymRepository { get; }
        IOnlineTrainingRepository OnlineTrainingRepository { get; }
        IOnlineTrainingSubscriptionRepository OnlineTrainingSubscriptionRepository { get; }
        Task<bool> SaveChangesAsync();
    }
}
