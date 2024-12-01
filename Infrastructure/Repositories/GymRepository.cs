using Core.Entities.GymEntities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IQueryable<Gym>> GetGymsQueryAsync(string? city, string? governorate, string? gymName, string? sortBy)
        {
            var query = context.Gyms!
                .Include("Ratings")
                .Include("Owner")
                .Include(g => g.GymSubscriptions)
                .AsQueryable();

            // Filtering logic
            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(x => x.City == city);

            if (!string.IsNullOrWhiteSpace(governorate))
                query = query.Where(x => x.Governorate == governorate);

            if (!string.IsNullOrWhiteSpace(gymName))
                query = query.Where(x => x.GymName.Contains(gymName));

            // Sorting logic
            query = sortBy switch
            {
                "rating" => query.OrderByDescending(g => g.Ratings!.Any() ? g.Ratings!.Average(r => r.RatingValue) : 0),
                "highestPrice" => query.OrderByDescending(g => g.MonthlyPrice),
                "lowestPrice" => query.OrderBy(g => g.MonthlyPrice),
                _ => query.OrderByDescending(g => g.GymSubscriptions!.Count)
            };

            return await Task.FromResult(query); // Return queryable data
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
