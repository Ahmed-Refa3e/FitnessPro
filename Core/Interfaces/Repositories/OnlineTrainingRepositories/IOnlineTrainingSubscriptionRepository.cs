using Core.Entities.OnlineTrainingEntities;

namespace Core.Interfaces.Repositories.OnlineTrainingRepositories
{
    public interface IOnlineTrainingSubscriptionRepository : IGenericRepository<OnlineTrainingSubscription>
    {
        Task<IReadOnlyList<OnlineTrainingSubscription?>> GetByTraineeIdAsync(string traineeId);
        Task<IReadOnlyList<OnlineTrainingSubscription?>> GetByOnlineTrainingIdAsync(int onlineTrainingId);
    }
}
