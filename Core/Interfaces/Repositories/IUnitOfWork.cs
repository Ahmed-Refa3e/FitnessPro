using Core.Interfaces.Repositories.OnlineTrainingRepositories;

namespace Core.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>() where T : class;
        IGymRepository GymRepository { get; }
        IOnlineTrainingRepository OnlineTrainingRepository { get; }
        IOnlineTrainingSubscriptionRepository OnlineTrainingSubscriptionRepository { get; }
        Task<bool> CompleteAsync();
    }
}
