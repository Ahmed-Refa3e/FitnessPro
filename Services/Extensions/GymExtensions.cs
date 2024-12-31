using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;

namespace Services.Extensions;

public static class GymExtensions
{
    public static GymResponseDto ToResponseDto(this Gym gym)
    {
        return gym == null
            ? throw new ArgumentNullException(nameof(gym))
            : new GymResponseDto
            {
                GymID = gym.GymID,
                GymName = gym.GymName,
                PictureUrl = gym.PictureUrl,
                Address = gym.Address,
                City = gym.City,
                Governorate = gym.Governorate,
                MonthlyPrice = gym.MonthlyPrice,
                AverageRating = (decimal)(gym.Ratings != null && gym.Ratings.Count != 0 ? gym.Ratings.Average(r => r.RatingValue) : 0),
                SubscriptionsCount = gym.GymSubscriptions?.Count ?? 0 // Calculate the number of subscriptions
            };
    }

    public static GymResponseDetailsDto ToResponseDetailsDto(this Gym gym)
    {
        return gym == null
            ? throw new ArgumentNullException(nameof(gym))
            : new GymResponseDetailsDto
            {
                GymID = gym.GymID,
                GymName = gym.GymName,
                PictureUrl = gym.PictureUrl,
                Address = gym.Address,
                City = gym.City,
                Governorate = gym.Governorate,
                MonthlyPrice = gym.MonthlyPrice,
                YearlyPrice = gym.YearlyPrice,
                FortnightlyPrice = gym.FortnightlyPrice,
                SessionPrice = gym.SessionPrice,
                PhoneNumber = gym.PhoneNumber,
                Description = gym.Description,
                CoachID = gym.CoachID,
                CoachFullName = $"{gym.Owner?.FirstName} {gym.Owner?.LastName}",
                CoachProfilePictureUrl = gym.Owner?.ProfilePictureUrl,
                AverageRating = (decimal)(gym.Ratings != null && gym.Ratings.Count != 0 ? gym.Ratings.Average(r => r.RatingValue) : 0),
                SubscriptionsCount = gym.GymSubscriptions?.Count ?? 0 // Calculate the number of subscriptions
            };

    }

    public static Gym ToEntity(this CreateGymDTO CreateGymDTO)
    {
        return CreateGymDTO == null
            ? throw new ArgumentNullException(nameof(CreateGymDTO))
            : new Gym
            {
                GymName = CreateGymDTO.GymName,
                PictureUrl = CreateGymDTO.PictureUrl,
                Address = CreateGymDTO.Address,
                City = CreateGymDTO.City,
                Governorate = CreateGymDTO.Governorate,
                MonthlyPrice = CreateGymDTO.MonthlyPrice,
                YearlyPrice = CreateGymDTO.YearlyPrice,
                FortnightlyPrice = CreateGymDTO.FortnightlyPrice,
                SessionPrice = CreateGymDTO.SessionPrice,
                PhoneNumber = CreateGymDTO.PhoneNumber,
                Description = CreateGymDTO.Description,
            };
    }
}
