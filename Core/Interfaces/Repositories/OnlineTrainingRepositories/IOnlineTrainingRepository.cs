using Core.Entities.OnlineTrainingEntities;

namespace Core.Interfaces.Repositories.OnlineTrainingRepositories
{
    public interface IOnlineTrainingRepository
    {
        Task<OnlineTraining?> GetByCoachIdAsync(string coachId);

    }
}
