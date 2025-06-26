using Core.Entities.OnlineTrainingEntities;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.OnlineTrainingRepositories
{
    public class OnlineTrainingRepository(FitnessContext context) : GenericRepository<OnlineTraining>(context), IOnlineTrainingRepository
    {
        public async Task<IReadOnlyList<OnlineTraining?>> GetGroupTrainingByCoachIdAsync(string coachId)
        {
            return await GetQueryable()
                .Where(ot => ot.CoachID == coachId && ot.TrainingType == "Group")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<OnlineTraining?>> GetPrivateTrainingByCoachIdAsync(string coachId)
        {
            return await GetQueryable()
                .Where(ot => ot.CoachID == coachId && ot.TrainingType == "Private")
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
