using Core.Entities.GymEntities;
using Infrastructure.Data;

namespace Infrastructure.Repositories.GymRepositories
{
    public class GymRatingRepository(FitnessContext context) : GenericRepository<GymRating>(context)
    {
    }
}
