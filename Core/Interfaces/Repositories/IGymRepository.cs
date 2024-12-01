using Core.Entities.GymEntities;

namespace Core.Interfaces.Repositories
{
    public interface IGymRepository
    {
        IQueryable<Gym> GetQueryable();
        Task<List<Gym>> ExecuteQueryAsync(IQueryable<Gym> query);
        Task<Gym?> GetGymByIdAsync(int id);
        Task<IReadOnlyList<string>> GetCitiesAsync();
        void AddGym(Gym Gym);
        void UpdateGym(Gym Gym);
        void DeleteGym(Gym Gym);
        bool GymExists(int id);
        Task<bool> SaveChangesAsync();
    }
}
