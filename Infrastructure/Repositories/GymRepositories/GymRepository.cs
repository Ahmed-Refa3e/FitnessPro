using Core.Entities.GymEntities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.GymRepositories
{
    public class GymRepository : GenericRepository<Gym>, IGymRepository
    {
        private readonly FitnessContext _context;

        public GymRepository(FitnessContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Gym?> GetByCoachIdAsync(string coachId)
        {
            return await _context.Set<Gym>()
                .FirstOrDefaultAsync(g => g.CoachID == coachId);
        }

        public async Task<IReadOnlyList<string>> GetCitiesAsync()
        {
            return await _context.Set<Gym>()
                .Select(g => g.City)
                .Distinct()
                .ToListAsync();
        }

        //override GetByIdAsync  
        public override async Task<Gym?> GetByIdAsync(int id)
        {
            return await _context.Set<Gym>()
                .Include(g => g.Owner)
                .Include(g => g.GymSubscriptions)
                .Include(g => g.Ratings)
                .FirstOrDefaultAsync(g => g.GymID == id);
        }
    }
}
