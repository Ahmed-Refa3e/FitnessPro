using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Core.Entities.Identity;
using Core.Helpers;

namespace Core.Interfaces.Services
{
    public interface IGymService
    {
        Task<PagedResult<GymResponseDto>> GetGymsAsync(GetGymDTO GymDTO);
        Task<Gym?> GetGymByIdAsync(int id);
        Task<IReadOnlyList<string>> GetCitiesAsync();
        Task<bool> CreateGymAsync(CreateGymDTO CreateGymDTO,ApplicationUser user);
        Task<bool> UpdateGymAsync(int id, UpdateGymDTO Gym);
        Task<bool> DeleteGymAsync(int id);
        Task<Gym?> GetGymByCoachIdAsync(string CoachId);
        Task<List<GymResponseDto>> GetNearbyGymsAsync(GetNearbyGymsDTO dto);
        Task<bool> UpdateGymLocationAsync(int gymId, double latitude, double longitude);
        Task<bool> AddGymLocationAsync(int gymId, double latitude, double longitude);

    }
}
