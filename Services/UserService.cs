using Core.DTOs.GeneralDTO;
using Core.DTOs.OnlineTrainingDTO;
using Core.DTOs.UserDTO;
using Core.Entities.Identity;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class UserService(IUserRepository repository, UserManager<ApplicationUser> _userManager) : IUserService
    {
        private const int MaxPageSize = 50;

        public async Task<PagedResult<CoachResponseDTO>> GetAllCoachesAsync(GetCoachesDTO getCoachesDTO)
        {
            var query = repository.GetAll().OfType<Coach>();

            if(!string.IsNullOrEmpty(getCoachesDTO.CoachName))
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
                case "firstname":
                    query = query.OrderBy(e => e.FirstName);
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
                        , includeProperties: "OnlineTrainings"
           );
            if (user is null)
            {
                response.IsSuccess = false;
                response.Data = "User Not Found.";
                return response;
            }

            var coach = (Coach)user;

            var UserDto = new GetCoachDetailsDTO
            {
                Id = CoachId,
                FullName = $"{user.FirstName} {user.LastName}",
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
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
                    TrainingType = trining.TrainingType
                }).ToList() ?? new List<GetOnlineTrainingDTO>()
            };

            response.IsSuccess = true;
            response.Data = UserDto;
            return response;
        }

        public GetProfileDTO GetProfileDetails(ApplicationUser user)
        {
            var UserDto = new GetProfileDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
                Gender = user.Gender,
                DateOfBirth = user.JoinedDate,
                JoinedDate = user.JoinedDate
            };
            return UserDto;
        }

        public async Task<Generalresponse> UpdateProfileDetailsAsync(UpdateProfileDTO profileDTO, ApplicationUser user)
        {
            Generalresponse response = new Generalresponse();

            user.FirstName = profileDTO.FirstName;
            user.LastName = profileDTO.LastName;
            user.Gender = profileDTO.Gender;
            user.DateOfBirth = profileDTO.DateOfBirth;
            user.Bio = profileDTO.Bio;

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

        public async Task<Generalresponse> ChangeProfilePictureAsync(IFormFile profilePicture, ApplicationUser user)
        {
            Generalresponse generalresponse = new Generalresponse();

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                var oldFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                                               "Infrastructure", user.ProfilePictureUrl.TrimStart('/'));

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            var newImagePath = await ImageHelper.SaveImageAsync(profilePicture, "ProfilePictures");
            user.ProfilePictureUrl = newImagePath;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                generalresponse.IsSuccess = true;
                generalresponse.Data = "Your profilePicture has been changed";
                return generalresponse;
            }
            generalresponse.IsSuccess = false;
            generalresponse.Data = result.Errors.Select(e => e.Description).ToList();
            return generalresponse;
        }

        public Generalresponse DeleteProfilePictureAsync(ApplicationUser user)
        {
            Generalresponse generalresponse = new Generalresponse();
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                var oldFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                                               "Infrastructure", user.ProfilePictureUrl.TrimStart('/'));

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                generalresponse.IsSuccess = true;
                generalresponse.Data = "Your profile picture has been removed successfully.";
                return generalresponse;
            }
            generalresponse.IsSuccess = false;
            generalresponse.Data = "No profile picture found.";
            return generalresponse;
        }
    }
}
