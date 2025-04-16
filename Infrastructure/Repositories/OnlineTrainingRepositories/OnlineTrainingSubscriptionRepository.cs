using Core.Entities.OnlineTrainingEntities;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories.OnlineTrainingRepositories
{
    public class OnlineTrainingSubscriptionRepository(FitnessContext context) : GenericRepository<OnlineTrainingSubscription>(context)
        ,IOnlineTrainingSubscriptionRepository
    {

    }
}
