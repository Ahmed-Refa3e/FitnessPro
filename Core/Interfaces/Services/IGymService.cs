using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Core.Helpers;

namespace Core.Interfaces.Services
{
    public interface IGymService
    {
        Task<PagedResult<GymResponseDto>> GetGymsAsync(GetGymDTO GymDTO);
        Task<Gym?> GetGymByIdAsync(int id);
        Task<IReadOnlyList<string>> GetCitiesAsync();
        Task<bool> CreateGymAsync(Gym Gym);
        Task<bool> UpdateGymAsync(int id, Gym Gym);
        Task<bool> DeleteGymAsync(int id);
    }
}
