using Core.DTOs;
using Core.Entities.GymEntities;
using Core.Helpers;

namespace Core.Interfaces
{
    public interface IGymRepository
    {
        Task<PagedResult<GymResponseDto>> GetGymsAsync(string? city, string? governorate, string? gymName, int pageNumber, int pageSize, string? sortBy);
        Task<Gym?> GetGymByIdAsync(int id);
        Task<IReadOnlyList<string>> GetCitiesAsync();
        void AddGym(Gym Gym);
        void UpdateGym(Gym Gym);
        void DeleteGym(Gym Gym);
        bool GymExists(int id);
        Task<bool> SaveChangesAsync();
    }
}
