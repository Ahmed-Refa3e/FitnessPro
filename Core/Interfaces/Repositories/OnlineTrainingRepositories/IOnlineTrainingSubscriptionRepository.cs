using Core.Entities.OnlineTrainingEntities;

namespace Core.Interfaces.Repositories.OnlineTrainingRepositories
{
    public interface IOnlineTrainingSubscriptionRepository : IGenericRepository<OnlineTrainingSubscription>
    {
        Task<IReadOnlyList<OnlineTrainingSubscription?>> GetByTraineeIdAsync(string traineeId);
        Task<IReadOnlyList<OnlineTrainingSubscription?>> GetByOnlineTrainingIdAsync(int onlineTrainingId);
        Task<bool> PaymentIntentExistsAsync(string paymentIntentId);
        Task<bool> HasActiveSubscriptionAsync(string traineeId, int trainingId);

    }
}
