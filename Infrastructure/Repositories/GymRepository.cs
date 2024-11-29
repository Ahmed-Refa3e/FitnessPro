using Core.DTOs;
using Core.Entities.GymEntities;
using Core.Helpers;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.Repositories
{
    public class GymRepository(FitnessContext context) : IGymRepository
    {
        private const int MaxPageSize = 50;
        public void AddGym(Gym Gym)
        {
            context.Gyms!.Add(Gym);
        }

        public void DeleteGym(Gym Gym)
        {
            context.Gyms!.Remove(Gym);
        }

        public async Task<IReadOnlyList<string>> GetCitiesAsync()
        {
            return await context.Gyms!.Select(x => x.City)
           .Distinct()
           .OrderBy(x => x)
           .ToListAsync();
        }

        public async Task<Gym?> GetGymByIdAsync(int id)
        {
            return await context.Gyms!.FindAsync(id);
        }

        public async Task<PagedResult<GymResponseDto>> GetGymsAsync(string? city, string? governorate,
            string? gymName, int pageNumber, int pageSize, string? sortBy)
        {
            {
                // Build the query
                var query = context.Gyms!
                    .Include("Ratings")
                    .Include("Owner")
                    .Include(g => g.GymSubscriptions) // Include subscriptions for sorting
                    .AsQueryable();

                // Filter by City
                if (!string.IsNullOrWhiteSpace(city))
                    query = query.Where(x => x.City == city);

                // Filter by Governorate
                if (!string.IsNullOrWhiteSpace(governorate))
                    query = query.Where(x => x.Governorate == governorate);

                // Filter by GymName
                if (!string.IsNullOrWhiteSpace(gymName))
                    query = query.Where(x => x.GymName.Contains(gymName));

                // Sorting (default: number of subscriptions)
                query = sortBy switch
                {
                    "rating" => query.OrderByDescending(g => g.Ratings!.Any() ? g.Ratings!.Average(r => r.RatingValue) : 0), // Sort by highest rating
                    "highestPrice" => query.OrderByDescending(g => g.MonthlyPrice), // Sort by highest monthly price
                    "lowestPrice" => query.OrderBy(g => g.MonthlyPrice), // Sort by lowest monthly price
                    _ => query.OrderByDescending(g => g.GymSubscriptions!.Count) // Default: Sort by number of subscriptions
                };

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
        }

        public bool GymExists(int id)
        {
            return context.Gyms!.Any(x => x.GymID == id);
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
