using Core.Entities.OnlineTrainingEntities;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.OnlineTrainingRepositories
{
    public class OnlineTrainingSubscriptionRepository(FitnessContext context) : GenericRepository<OnlineTrainingSubscription>(context)
        , IOnlineTrainingSubscriptionRepository
    {
        public async Task<IReadOnlyList<OnlineTrainingSubscription?>> GetByOnlineTrainingIdAsync(int onlineTrainingId)
        {
            IQueryable<OnlineTrainingSubscription> query = GetQueryable()
                .Where(x => x.OnlineTrainingId == onlineTrainingId)
                .Include(x => x.OnlineTraining)
                .Include(x => x.Trainee);
            return await ExecuteQueryAsync(query);
        }

        public async Task<IReadOnlyList<OnlineTrainingSubscription?>> GetByTraineeIdAsync(string traineeId)
        {
            IQueryable<OnlineTrainingSubscription> query = GetQueryable()
                .Where(x => x.TraineeID == traineeId)
                .Include(x => x.OnlineTraining)
                .Include(x => x.Trainee);
            return await ExecuteQueryAsync(query);
        }

        public override Task<OnlineTrainingSubscription?> GetByIdAsync(int id)
        {
            IQueryable<OnlineTrainingSubscription> query = GetQueryable()
                .Where(x => x.Id == id)
                .Include(x => x.OnlineTraining)
                .Include(x => x.Trainee);
            return ExecuteQueryAsync(query).ContinueWith(t => t.Result.FirstOrDefault());
        }

        public async Task<bool> PaymentIntentExistsAsync(string paymentIntentId)
        {
            return await context.OnlineTrainingSubscriptions!
                .AnyAsync(s => s.StripePaymentIntentId == paymentIntentId);
        }

        public async Task<bool> HasActiveSubscriptionAsync(string traineeId, int trainingId)
        {
            return await context.OnlineTrainingSubscriptions!
                .AnyAsync(s =>
                    s.TraineeID == traineeId &&
                    s.OnlineTrainingId == trainingId &&
                    s.IsActive &&
                    s.EndDate > DateTime.Now);
        }

    }
}
