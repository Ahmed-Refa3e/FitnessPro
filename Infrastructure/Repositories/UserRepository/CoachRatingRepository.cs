using Core.Entities.OnlineTrainingEntities;
using Infrastructure.Data;

namespace Infrastructure.Repositories.UserRepository
{
    public class CoachRatingRepository(FitnessContext context) : GenericRepository<CoachRating>(context)
    {
    }
}
