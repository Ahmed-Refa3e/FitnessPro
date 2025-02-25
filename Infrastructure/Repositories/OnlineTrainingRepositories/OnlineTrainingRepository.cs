using Core.Entities.OnlineTrainingEntities;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.OnlineTrainingRepositories
{
    public class OnlineTrainingRepository(FitnessContext context) : GenericRepository<OnlineTraining>(context), IOnlineTrainingRepository
    {
        public async Task<OnlineTraining?> GetByCoachIdAsync(string coachId)
        {
            return await GetQueryable().FirstOrDefaultAsync(t => t.CoachID == coachId);
        }
    }
}
