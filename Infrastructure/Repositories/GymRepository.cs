using Core.Entities.GymEntities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public class GymRepository(FitnessContext context) : IGymRepository
{
    public IQueryable<Gym> GetQueryable()
    {
        return context.Gyms!
            .Include(g => g.Ratings)
            .Include(g => g.Owner)
            .Include(g => g.GymSubscriptions)
            .AsQueryable();
    }

    public async Task<List<Gym>> ExecuteQueryAsync(IQueryable<Gym> query)
    {
        return await query.ToListAsync();
    }

    public async Task<Gym?> GetGymByIdAsync(int id)
    {
        return await context.Gyms!.FindAsync(id);
    }

    public async Task<IReadOnlyList<string>> GetCitiesAsync()
    {
        return await context.Gyms!
            .Select(x => x.City)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
    }

    public void AddGym(Gym Gym)
    {
        context.Gyms!.Add(Gym);
    }

    public void UpdateGym(Gym Gym)
    {
        context.Entry(Gym).State = EntityState.Modified;
    }

    public void DeleteGym(Gym Gym)
    {
        context.Gyms!.Remove(Gym);
    }

    public bool GymExists(int id)
    {
        return context.Gyms!.Any(x => x.GymID == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
