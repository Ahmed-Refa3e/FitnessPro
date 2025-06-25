using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Core.Entities.Identity;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Services.Extensions;

namespace Services;

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
            .Take(pageSize)
            .AsNoTracking();

        // Execute the query using the repository
        IReadOnlyList<Gym> gyms = await repository.ExecuteQueryAsync(paginatedQuery);

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

    public async Task<bool> UpdateGymAsync(int id, UpdateGymDTO dto)
    {
        var existingGym = await repository.GetByIdAsync(id);
        if (existingGym == null)
            return false;

        existingGym.GymName = dto.GymName;
        existingGym.Address = dto.Address;
        existingGym.City = dto.City;
        existingGym.Governorate = dto.Governorate;
        existingGym.MonthlyPrice = dto.MonthlyPrice;
        existingGym.Description = dto.Description;
        existingGym.PictureUrl = dto.PictureUrl;
        existingGym.PhoneNumber = dto.PhoneNumber;
        existingGym.SessionPrice = dto.SessionPrice;
        existingGym.FortnightlyPrice = dto.FortnightlyPrice;
        existingGym.YearlyPrice = dto.YearlyPrice;

        try
        {
            return await repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating gym: {ex.Message}");
            return false;
        }
    }


    public async Task<bool> DeleteGymAsync(int id)
    {
        var gym = await repository.GetByIdAsync(id);
        if (gym == null) return false;

        repository.Remove(gym);
        return await repository.SaveChangesAsync();
    }

    public async Task<Gym?> GetGymByCoachIdAsync(string CoachId)
    {
        return await repository.GetByCoachIdAsync(CoachId);
    }
    public async Task<List<GymResponseDto>> GetNearbyGymsAsync(GetNearbyGymsDTO dto)
    {
        const double MaxDistanceKm = 5;

        BoundingBox(dto, MaxDistanceKm, out double minLat, out double maxLat, out double minLng, out double maxLng);

        var query = repository.GetQueryable()
            .Where(g => g.Latitude >= minLat && g.Latitude <= maxLat
                     && g.Longitude >= minLng && g.Longitude <= maxLng
                     && g.Latitude != 0 && g.Longitude != 0)
            .Include(g => g.Ratings)
            .Include(g => g.Owner)
            .AsNoTracking();

        var gyms = await query.ToListAsync();
        if (gyms.Count == 0) return [];

        var nearbyGyms = gyms
            .Where(g => CalculateDistance(g.Latitude, g.Longitude, dto.Latitude, dto.Longitude) <= MaxDistanceKm)
            .Select(g => g.ToResponseDto())
            .ToList();

        return nearbyGyms;
    }

    private static void BoundingBox(GetNearbyGymsDTO dto, double MaxDistanceKm, out double minLat, out double maxLat, out double minLng, out double maxLng)
    {
        double lat = dto.Latitude;
        double lng = dto.Longitude;

        double deltaLat = MaxDistanceKm / 111.0;
        double deltaLng = MaxDistanceKm / (111.0 * Math.Cos(lat * (Math.PI / 180)));

        minLat = lat - deltaLat;
        maxLat = lat + deltaLat;
        minLng = lng - deltaLng;
        maxLng = lng + deltaLng;
    }

    public async Task<bool> AddGymLocationAsync(int gymId, double latitude, double longitude)
    {
        var gym = await repository.GetByIdAsync(gymId);
        if (gym == null || (gym.Latitude != 0 || gym.Longitude != 0)) return false;

        gym.Latitude = latitude;
        gym.Longitude = longitude;

        return await repository.SaveChangesAsync();
    }

    public async Task<bool> UpdateGymLocationAsync(int gymId, double latitude, double longitude)
    {
        var gym = await repository.GetByIdAsync(gymId);
        if (gym == null) return false;
        
        gym.Latitude = latitude;
        gym.Longitude = longitude;

        repository.Update(gym);
        return await repository.SaveChangesAsync();
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Radius of Earth in km
        var latRad1 = lat1 * (Math.PI / 180);
        var latRad2 = lat2 * (Math.PI / 180);
        var deltaLat = (lat2 - lat1) * (Math.PI / 180);
        var deltaLon = (lon2 - lon1) * (Math.PI / 180);

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(latRad1) * Math.Cos(latRad2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}
