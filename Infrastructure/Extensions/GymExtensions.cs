using Core.DTOs;
using Core.Entities.GymEntities;

namespace Infrastructure.Extensions;

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
                Country = gym.Country,
                MonthlyPrice = gym.MonthlyPrice,
                YearlyPrice = gym.YearlyPrice,
                FortnightlyPrice = gym.FortnightlyPrice,
                SessionPrice = gym.SessionPrice,
                PhoneNumber = gym.PhoneNumber,
                Description = gym.Description,
                CoachID = gym.CoachID,
                CoachFullName = $"{gym.Owner?.FirstName} {gym.Owner?.LastName}",
                CoachProfilePictureUrl = gym.Owner?.ProfilePictureUrl,
                AverageRating = (decimal)(gym.Ratings != null && gym.Ratings.Count != 0 ? gym.Ratings.Average(r => r.RatingValue) : 0)
            };
    }
}
