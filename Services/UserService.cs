using Core.DTOs.GeneralDTO;
using Core.DTOs.OnlineTrainingDTO;
using Core.DTOs.PostDTO;
using Core.DTOs.UserDTO;
using Core.Entities.Identity;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class UserService(IUserRepository repository, UserManager<ApplicationUser> _userManager) : IUserService
    {
        private const int MaxPageSize = 50;

        public async Task<PagedResult<CoachResponseDTO>> GetAllCoachesAsync(GetCoachesDTO getCoachesDTO)
        {
            var query = repository.GetAll(includeProperties: "Ratings").OfType<Coach>();

            if (!string.IsNullOrEmpty(getCoachesDTO.CoachName))
            {
                query = query
                    .Where(e => EF.Functions.Like(e.FirstName + " " + e.LastName, $"%{getCoachesDTO.CoachName}%"));
            }

            if (getCoachesDTO.MinRating.HasValue)
            {
                query = query.Where(e => e.Ratings != null && e.Ratings.Any()
                        && e.Ratings.Average(e => e.Rating) >= getCoachesDTO.MinRating.Value);
            }

            if (getCoachesDTO.MaxRating.HasValue)
            {
                query = query.Where(e => e.Ratings != null && e.Ratings.Any()
                         && e.Ratings.Average(e => e.Rating) <= getCoachesDTO.MaxRating.Value);
            }

            if (string.IsNullOrWhiteSpace(getCoachesDTO.SortBy))
            {
                getCoachesDTO.SortBy = "joineddate";
            }

            switch (getCoachesDTO.SortBy.ToLower())
            {
                case "coachname":
                    query = query.OrderBy(e => e.FirstName).ThenBy(e => e.LastName);
                    break;
                case "rating":
                    query = query.OrderByDescending(e => e.Ratings != null && e.Ratings.Any()
                                ? e.Ratings.Average(k => k.Rating)
                                : 0);
                    break;
                case "joineddate":
                default:
                    query = query.OrderByDescending(c => c.JoinedDate);
                    break;
            }

            var pageSize = getCoachesDTO.PageSize > MaxPageSize ? MaxPageSize : getCoachesDTO.PageSize;
            var totalCount = await query.CountAsync();

            var PaginatedCoaches = await query
                                            .Skip((getCoachesDTO.PageNumber - 1) * getCoachesDTO.PageSize)
                                            .Take(getCoachesDTO.PageSize).ToListAsync();

            var Coaches = new List<CoachResponseDTO>();
            foreach (var coach in PaginatedCoaches)
            {
                var newcoach = new CoachResponseDTO()
                {
                    Id = coach.Id,
                    FullName = $"{coach.FirstName} {coach.LastName}",
                    Bio = coach.Bio,
                    Gender = coach.Gender,
                    JoinedDate = coach.JoinedDate,
                    ProfilePictureUrl = coach.ProfilePictureUrl,
                    Rating = coach.Ratings != null && coach.Ratings.Any()
                             ? coach.Ratings.Average(r => r.Rating)
                             : 0
                };
                Coaches.Add(newcoach);
            }

            return new PagedResult<CoachResponseDTO>(Coaches, totalCount, getCoachesDTO.PageNumber, pageSize);
        }

        public async Task<Generalresponse> GetCoachDetailsAsync(string CoachId)
        {
            Generalresponse response = new Generalresponse();

            var user = await repository.GetAsync(e => e.Id == CoachId
                        , includeProperties: "OnlineTrainings,Posts,Ratings"
           );
            if (user is null)
            {
                response.IsSuccess = false;
                response.Data = "User Not Found.";
                return response;
            }
            if (user is Trainee trainee)
            {
                response.IsSuccess = false;
                response.Data = "This is Trainee.";
                return response;
            }

            var coach = (Coach)user;

            var UserDto = new GetCoachDetailsDTO
            {
                Id = CoachId,
                FullName = $"{user.FirstName} {user.LastName}",
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = coach.Bio,
                Gender = user.Gender,
                JoinedDate = user.JoinedDate,
                OnlineTrainings = coach.OnlineTrainings?.Select(trining => new GetOnlineTrainingDTO
                {
                    Id = trining.Id,
                    Description = trining.Description,
                    DurationOfSession = trining.DurationOfSession,
                    NoOfSessionsPerWeek = trining.NoOfSessionsPerWeek,
                    Price = trining.Price,
                    Title = trining.Title,
                    TrainingType = trining.TrainingType.ToString()
                }).ToList() ?? new List<GetOnlineTrainingDTO>(),
                CoachPosts = coach.Posts?.Select(post => new CoachPostSummaryDTO
                {
                    Id = post.Id,
                    Content = post.Content,
                    ImageUrl = post.PictureUrls?.FirstOrDefault()?.Url,
                    CreatedAt = post.CreatedAt
                }).ToList() ?? new List<CoachPostSummaryDTO>()
            };

            double? rating = coach.Ratings?.Any() == true
            ? coach.Ratings.Average(r => r.Rating)
            : null;
            UserDto.Rating = rating;

            response.IsSuccess = true;
            response.Data = UserDto;
            return response;
        }

        public GetProfileDTO GetProfileDetails(ApplicationUser user)
        {
            if (user is Coach coach)
            {
                return new CoachProfileDTO
                {
                    Id = coach.Id,
                    FirstName = coach.FirstName,
                    LastName = coach.LastName,
                    ProfilePictureUrl = coach.ProfilePictureUrl,
                    Gender = coach.Gender,
                    DateOfBirth = coach.DateOfBirth,
                    JoinedDate = coach.JoinedDate,
                    Bio = coach.Bio
                };
            }

            return new GetProfileDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                JoinedDate = user.JoinedDate
            };
        }

        public async Task<Generalresponse> UpdateProfileDetailsAsync(UpdateProfileDTO profileDTO, ApplicationUser user)
        {
            Generalresponse response = new Generalresponse();

            if (user is not Coach && !string.IsNullOrEmpty(profileDTO.Bio))
            {
                response.IsSuccess = false;
                response.Data = "Users cannot update their bio.";
                return response;
            }

            user.FirstName = profileDTO.FirstName;
            user.LastName = profileDTO.LastName;
            user.Gender = profileDTO.Gender;
            user.DateOfBirth = profileDTO.DateOfBirth;

            if (user is Coach coach)
            {
                coach.Bio = profileDTO.Bio;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                response.IsSuccess = false;
                response.Data = result.Errors.Select(e => e.Description).ToList();
                return response;
            }
            response.IsSuccess = true;
            response.Data = "Profile updated successfully";
            return response;
        }

        public async Task<bool> CheckUserStatusAsync(ApplicationUser user)
        {
            var userfromDb = await repository.GetAsync(e => e.Id == user.Id,
                includeProperties: "OnlineTrainings,Shops,Gym");

            if (userfromDb == null)
            {
                return false;
            }

            if (userfromDb is Coach coach)
            {
                return (coach.Gym != null) ||
                       (coach.OnlineTrainings?.Any() == true) ||
                       (coach.Shops?.Any() == true);
            }

            return false;
        }
    }
}
