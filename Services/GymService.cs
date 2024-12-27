using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Core.Entities.Identity;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Services.Extensions;

namespace Services
{
    public class GymService(IGymRepository repository) : IGymService
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

        public async Task<bool> CreateGymAsync(CreateGymDTO CreateGymDTO, ApplicationUser user)
        {
            var GymByCoachId = repository.GetByCoachIdAsync(user.Id);
            if (GymByCoachId.Result != null)
            {
                return false;
            }
            // Map DTO to entity
            var gym = CreateGymDTO.ToEntity();

            //Assign the current user's ID as the CoachID
            gym.CoachID = user.Id;

            repository.Add(gym);
            return await repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateGymAsync(int id, UpdateGymDTO gymDto)
        {
            // Check if the gym exists
            var existingGym = await repository.GetByIdAsync(id);
            if (existingGym == null)
            {
                return false; // Gym not found
            }

            // Update the properties of the gym entity
            existingGym.GymName = gymDto.GymName;
            existingGym.Address = gymDto.Address;
            existingGym.City = gymDto.City;
            existingGym.Governorate = gymDto.Governorate;
            existingGym.MonthlyPrice = gymDto.MonthlyPrice;
            existingGym.Description = gymDto.Description;
            existingGym.PictureUrl = gymDto.PictureUrl;
            existingGym.PhoneNumber = gymDto.PhoneNumber;
            existingGym.SessionPrice = gymDto.SessionPrice;
            existingGym.FortnightlyPrice = gymDto.FortnightlyPrice;
            existingGym.YearlyPrice = gymDto.YearlyPrice;

            // Update the entity in the repository
            repository.Update(existingGym);

            // Save changes to the database
            return await repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteGymAsync(int id)
        {
            var gym = await repository.GetByIdAsync(id);
            if (gym == null) return false;

            repository.Delete(gym);
            return await repository.SaveChangesAsync();
        }

        public async Task<Gym?> GetGymByCoachIdAsync(string CoachId)
        {
            return await repository.GetByCoachIdAsync(CoachId);
        }
    }
}
