using Core.Entities.GymEntities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GymRepository(FitnessContext context) : GenericRepository<Gym>(context), IGymRepository
    {
        public async Task<IReadOnlyList<string>> GetCitiesAsync()
        {
            return await context.Set<Gym>()
                .Select(g => g.City)
                .Distinct()
                .ToListAsync();
        }
    }
}
