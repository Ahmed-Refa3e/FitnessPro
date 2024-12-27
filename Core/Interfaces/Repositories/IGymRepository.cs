using Core.Entities.GymEntities;

namespace Core.Interfaces.Repositories
{
    public interface IGymRepository : IGenericRepository<Gym>
    {
        Task<IReadOnlyList<string>> GetCitiesAsync();
        Task<Gym?> GetByCoachIdAsync(string coachId);
    }
}
