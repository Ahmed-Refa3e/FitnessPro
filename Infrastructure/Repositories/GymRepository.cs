using Core.DTOs;
using Core.Entities.GymEntities;
using Core.Helpers;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GymRepository(FitnessContext context) : IGymRepository
    {
        private const int MaxPageSize = 50;
        public void AddGym(Gym Gym)
        {
            context.Gyms.Add(Gym);
        }

        public void DeleteGym(Gym Gym)
        {
            context.Gyms.Remove(Gym);
        }

        public async Task<IReadOnlyList<string>> GetCitiesAsync()
        {
            return await context.Gyms.Select(x => x.City)
           .Distinct()
           .OrderBy(x => x)
           .ToListAsync();
        }

        public async Task<Gym?> GetGymByIdAsync(int id)
        {
            return await context.Gyms.FindAsync(id);
        }

        public async Task<PagedResult<GymResponseDto>> GetGymsAsync(string? City, int pageNumber, int pageSize)
        {
            var query = context.Gyms.Include("Ratings").Include("Owner").AsQueryable();

            if (!string.IsNullOrWhiteSpace(City))
                query = query.Where(x => x.City == City);

            // Ensure the pageSize does not exceed the maximum limit
            pageSize = (pageSize > MaxPageSize) ? MaxPageSize : pageSize;

            // Total record count
            var count = await query.CountAsync();

            // Apply pagination using Skip and Take
            var data = await query
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

            // Map the data to GymResponseDto
            var dtoData = data.Select(g => g.ToResponseDto()).ToList();

            return new PagedResult<GymResponseDto>(dtoData, count, pageNumber, pageSize);
        }

        public bool GymExists(int id)
        {
            return context.Gyms.Any(x => x.GymID == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void UpdateGym(Gym Gym)
        {
            context.Entry(Gym).State = EntityState.Modified;
        }
    }
}
