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
                AverageRating = Math.Round((decimal)(gym.Ratings != null && gym.Ratings.Count != 0 ? gym.Ratings.Average(r => r.RatingValue) : 0), 1),
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
                AverageRating = Math.Round((decimal)(gym.Ratings != null && gym.Ratings.Count != 0 ? gym.Ratings.Average(r => r.RatingValue) : 0), 1),
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

    public static GymRating ToEntity(this CreateGymRatingDTO createGymRatingDTO)
    {
        return createGymRatingDTO == null
            ? throw new ArgumentNullException(nameof(createGymRatingDTO))
            : new GymRating
            {
                GymID = createGymRatingDTO.GymID,
                Review = createGymRatingDTO.Review,
                RatingValue = createGymRatingDTO.RatingValue
            };
    }

    public static GymRatingResponseDTO ToResponseDto(this GymRating gymRating)
    {
        return gymRating == null
            ? throw new ArgumentNullException(nameof(gymRating))
            : new GymRatingResponseDTO
            {
                GymRatingID = gymRating.GymRatingID,
                RatingValue = gymRating.RatingValue,
                Review = gymRating.Review
            };
    }

}
