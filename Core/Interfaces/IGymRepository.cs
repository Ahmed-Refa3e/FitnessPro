using Core.Entities.GymEntities;
using Core.Helpers;

namespace Core.Interfaces
{
    public interface IGymRepository
    {
        Task<PagedResult<Gym>> GetGymsAsync(string? City, int pageNumber, int pageSize);
        Task<Gym?> GetGymByIdAsync(int id);
        Task<IReadOnlyList<string>> GetCitiesAsync();
        void AddGym(Gym Gym);
        void UpdateGym(Gym Gym);
        void DeleteGym(Gym Gym);
        bool GymExists(int id);
        Task<bool> SaveChangesAsync();
    }
}
