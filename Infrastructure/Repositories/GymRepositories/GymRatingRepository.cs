using Core.Entities.GymEntities;
using Infrastructure.Data;

namespace Infrastructure.Repositories.GymRepositories
{
    public class GymRatingRepository(FitnessContext context) : GenericRepository<GymRating>(context)
    {
        public async Task<object?> GetByConditionAsync(Func<object, bool> value)
        {
            var query = GetQueryable();
            var result = await ExecuteQueryAsync(query);
            return result.FirstOrDefault(value);
        }
    }
}
