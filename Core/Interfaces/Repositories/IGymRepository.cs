using Core.Entities.GymEntities;

namespace Core.Interfaces.Repositories
{
    public interface IGymRepository
    {
        Task<IQueryable<Gym>> GetGymsQueryAsync(string? city, string? governorate, string? gymName, string? sortBy);
        Task<Gym?> GetGymByIdAsync(int id);
        Task<IReadOnlyList<string>> GetCitiesAsync();
        void AddGym(Gym Gym);
        void UpdateGym(Gym Gym);
        void DeleteGym(Gym Gym);
        bool GymExists(int id);
        Task<bool> SaveChangesAsync();
    }
}
