using Core.Entities.OnlineTrainingEntities;

namespace Core.Interfaces.Repositories.OnlineTrainingRepositories
{
    public interface IOnlineTrainingRepository : IGenericRepository<OnlineTraining>
    {
        Task<IReadOnlyList<OnlineTraining?>> GetGroupTrainingByCoachIdAsync(string coachId);

        Task<IReadOnlyList<OnlineTraining?>> GetPrivateTrainingByCoachIdAsync(string coachId);

    }
}
