using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Core.Entities.Identity;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Services.Extensions;

namespace Services;

public class GymService(IUnitOfWork unitOfWork) : IGymService
{
    private const int MaxPageSize = 50;

    public async Task<PagedResult<GymResponseDto>> GetGymsAsync(GetGymDTO gymDTO)
    {
        var query = BuildBaseGymQuery();

        query = ApplyFilters(query, gymDTO);
        query = ApplySorting(query, gymDTO.SortBy);

        var pagedQuery = ApplyPagination(query, gymDTO.PageNumber, gymDTO.PageSize, out int pageSize);

        var totalCount = await query.CountAsync();
        var gyms = await unitOfWork.GymRepository.ExecuteQueryAsync(pagedQuery);
        var dtoData = gyms.Select(g => g.ToResponseDto()).ToList();

        return new PagedResult<GymResponseDto>(dtoData, totalCount, gymDTO.PageNumber, pageSize);
    }

    private IQueryable<Gym> BuildBaseGymQuery()
    {
        return unitOfWork.GymRepository.GetQueryable()
            .Include(g => g.GymSubscriptions)
            .Include(g => g.Ratings)
            .Include(g => g.Owner)
            .AsNoTracking();
    }

    private static IQueryable<Gym> ApplyFilters(IQueryable<Gym> query, GetGymDTO dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.City))
            query = query.Where(g => g.City == dto.City);

        if (!string.IsNullOrWhiteSpace(dto.Governorate))
            query = query.Where(g => g.Governorate == dto.Governorate);

        if (!string.IsNullOrWhiteSpace(dto.GymName))
            query = query.Where(g => g.GymName.Contains(dto.GymName));

        return query;
    }
    private static IQueryable<Gym> ApplySorting(IQueryable<Gym> query, string? sortBy)
    {
        return sortBy switch
        {
            "rating" => query.OrderByDescending(g =>
                g.Ratings!.Any() ? g.Ratings!.Average(r => r.RatingValue) : 0),

            "highestPrice" => query.OrderByDescending(g => g.MonthlyPrice),

            "lowestPrice" => query.OrderBy(g => g.MonthlyPrice),

            _ => query.OrderByDescending(g => g.GymSubscriptions!.Count)
        };
    }

    private static IQueryable<Gym> ApplyPagination(IQueryable<Gym> query, int pageNumber, int pageSize, out int safePageSize)
    {
        safePageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;

        return query
            .Skip((pageNumber - 1) * safePageSize)
            .Take(safePageSize);
    }


    public async Task<Gym?> GetGymByIdAsync(int id)
    {
        return await unitOfWork.GymRepository.GetByIdAsync(id);
    }

    public async Task<IReadOnlyList<string>> GetCitiesAsync()
    {
        return await unitOfWork.GymRepository.GetCitiesAsync();
    }

    public async Task<bool> CreateGymAsync(CreateGymDTO CreateGymDTO, ApplicationUser user)
    {
        var GymByCoachId = await unitOfWork.GymRepository.GetByCoachIdAsync(user.Id);
        if (GymByCoachId is not null)
        {
            return false;
        }
        var gym = CreateGymDTO.ToEntity();

        gym.CoachID = user.Id;

        unitOfWork.GymRepository.Add(gym);
        return await unitOfWork.CompleteAsync();
    }

    public async Task<bool> UpdateGymAsync(int id, UpdateGymDTO dto)
    {
        var existingGym = await unitOfWork.GymRepository.GetByIdAsync(id);
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
            return await unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating gym: {ex.Message}");
            return false;
        }
    }


    public async Task<bool> DeleteGymAsync(int id)
    {
        var gym = await unitOfWork.GymRepository.GetByIdAsync(id);
        if (gym == null) return false;

        unitOfWork.GymRepository.Remove(gym);
        return await unitOfWork.CompleteAsync();
    }

    public async Task<Gym?> GetGymByCoachIdAsync(string CoachId)
    {
        return await unitOfWork.GymRepository.GetByCoachIdAsync(CoachId);
    }
    public async Task<List<GymResponseDto>> GetNearbyGymsAsync(GetNearbyGymsDTO dto)
    {
        const double MaxDistanceKm = 5;

        BoundingBox(dto, MaxDistanceKm, out double minLat, out double maxLat, out double minLng, out double maxLng);

        var query = unitOfWork.GymRepository.GetQueryable()
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
        var gym = await unitOfWork.GymRepository.GetByIdAsync(gymId);
        if (gym == null || (gym.Latitude != 0 || gym.Longitude != 0)) return false;

        gym.Latitude = latitude;
        gym.Longitude = longitude;

        return await unitOfWork.CompleteAsync();
    }

    public async Task<bool> UpdateGymLocationAsync(int gymId, double latitude, double longitude)
    {
        var gym = await unitOfWork.GymRepository.GetByIdAsync(gymId);
        if (gym == null) return false;

        gym.Latitude = latitude;
        gym.Longitude = longitude;

        unitOfWork.GymRepository.Update(gym);
        return await unitOfWork.CompleteAsync();
    }
}
