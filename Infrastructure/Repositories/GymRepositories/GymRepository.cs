using Core.Entities.GymEntities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.GymRepositories
{
    public class GymRepository(FitnessContext context) : GenericRepository<Gym>(context), IGymRepository
    {
        public async Task<Gym?> GetByCoachIdAsync(string coachId)
        {
            return await context.Set<Gym>()
                .FirstOrDefaultAsync(g => g.CoachID == coachId);
        }

        public async Task<IReadOnlyList<string>> GetCitiesAsync()
        {
            return await context.Set<Gym>()
                .Select(g => g.City)
                .Distinct()
                .ToListAsync();
        }

        //override GetByIdAsync
        public override async Task<Gym?> GetByIdAsync(int id)
        {
            return await context.Set<Gym>()
                .Include(g => g.Owner)
                .Include(g => g.GymSubscriptions)
                .Include(g => g.Ratings)
                .FirstOrDefaultAsync(g => g.GymID == id);
        }
    }
}
