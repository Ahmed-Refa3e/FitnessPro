using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Core.Entities.Identity;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Extensions;

namespace Services
{
    public class GymService(IGymRepository repository, SignInManager<ApplicationUser> signInManager) : IGymService
    {
        private const int MaxPageSize = 50;

        public async Task<PagedResult<GymResponseDto>> GetGymsAsync(GetGymDTO GymDTO)
        {
            // Start with the base queryable from the repository
            var query = repository.GetQueryable();

            // Include related entities
            query = query
                .Include(g => g.GymSubscriptions)
                .Include(g => g.Ratings)
                .Include(g => g.Owner);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(GymDTO.City))
                query = query.Where(x => x.City == GymDTO.City);

            if (!string.IsNullOrWhiteSpace(GymDTO.Governorate))
                query = query.Where(x => x.Governorate == GymDTO.Governorate);

            if (!string.IsNullOrWhiteSpace(GymDTO.GymName))
                query = query.Where(x => x.GymName.Contains(GymDTO.GymName));

            // Apply sorting
            query = GymDTO.SortBy switch
            {
                "rating" => query.OrderByDescending(g => g.Ratings!.Any() ? g.Ratings!.Average(r => r.RatingValue) : 0),
                "highestPrice" => query.OrderByDescending(g => g.MonthlyPrice),
                "lowestPrice" => query.OrderBy(g => g.MonthlyPrice),
                _ => query.OrderByDescending(g => g.GymSubscriptions!.Count)
            };

            // Apply pagination
            var pageSize = GymDTO.PageSize > MaxPageSize ? MaxPageSize : GymDTO.PageSize;
            var count = await query.CountAsync();
            var paginatedQuery = query
                .Skip((GymDTO.PageNumber - 1) * pageSize)
                .Take(pageSize);

            // Execute the query using the repository
            var gyms = await repository.ExecuteQueryAsync(paginatedQuery);

            // Map to DTOs
            var dtoData = gyms.Select(g => g.ToResponseDto()).ToList();

            return new PagedResult<GymResponseDto>(dtoData, count, GymDTO.PageNumber, pageSize);
        }


        public async Task<Gym?> GetGymByIdAsync(int id)
        {
            return await repository.GetByIdAsync(id);
        }

        public async Task<IReadOnlyList<string>> GetCitiesAsync()
        {
            return await repository.GetCitiesAsync();
        }

        public async Task<bool> CreateGymAsync(CreateGymDTO CreateGymDTO)
        {
            // Get the current user from UserManager
            var user = await signInManager.UserManager.GetUserAsync(signInManager.Context.User)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            // Map DTO to entity
            var gym = CreateGymDTO.ToEntity();

            // Assign the current user's ID as the CoachID
            gym.CoachID = user.Id;

            repository.Add(gym);
            return await repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateGymAsync(int id, Gym Gym)
        {
            if (await repository.ExistsAsync(id) || Gym.GymID != id)
                return false;

            repository.Update(Gym);
            return await repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteGymAsync(int id)
        {
            var gym = await repository.GetByIdAsync(id);
            if (gym == null) return false;

            repository.Delete(gym);
            return await repository.SaveChangesAsync();
        }
    }
}
