using Core.DTOs;
using Core.Entities.GymEntities;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class GymService(IGymRepository repository) : IGymService
    {
        private const int MaxPageSize = 50;

        public async Task<PagedResult<GymResponseDto>> GetGymsAsync(GetGymDTO GymDTO)
        {
            var query = await repository.GetGymsQueryAsync(
                GymDTO.City,
                GymDTO.Governorate,
                GymDTO.GymName,
                GymDTO.SortBy
            );

            // Pagination logic
            var pageSize = GymDTO.PageSize > MaxPageSize ? MaxPageSize : GymDTO.PageSize;
            var count = await query.CountAsync();

            var data = await query
                .Skip((GymDTO.PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map data to DTO
            var dtoData = data.Select(g => g.ToResponseDto()).ToList();

            return new PagedResult<GymResponseDto>(dtoData, count, GymDTO.PageNumber, pageSize);
        }

        public async Task<Gym?> GetGymByIdAsync(int id) => await repository.GetGymByIdAsync(id);

        public async Task<IReadOnlyList<string>> GetCitiesAsync() => await repository.GetCitiesAsync();

        public async Task<bool> CreateGymAsync(Gym Gym)
        {
            repository.AddGym(Gym);
            return await repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateGymAsync(int id, Gym Gym)
        {
            if (!repository.GymExists(id) || Gym.GymID != id)
                return false;

            repository.UpdateGym(Gym);
            return await repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteGymAsync(int id)
        {
            var gym = await repository.GetGymByIdAsync(id);
            if (gym == null) return false;

            repository.DeleteGym(gym);
            return await repository.SaveChangesAsync();
        }
    }
}
