using Core.DTOs.GymDTO;
using Core.DTOs.OnlineTrainingDTO;
using Core.DTOs.UserDTO;
using Core.Entities.Identity;
using Core.Entities.OnlineTrainingEntities;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IUserRepository repository, UserManager<ApplicationUser> _userManager)
        {
            this.repository = repository;
            this._userManager = _userManager;
        }

        public async Task<Generalresponse> GetAllCoachesAsync()
        {
            Generalresponse response = new Generalresponse();
            var coaches = await repository.GetAllAsync(
                    expression: user => user is Coach,
                    includeProperties: "Gym,OnlineTrainings"
             );

            var coachDtos = coaches.Select(coach => new GetCoachDTO
            {
                Id = coach.Id,
                FirstName = coach.FirstName,
                LastName = coach.LastName,
                ProfilePictureUrl = coach.ProfilePictureUrl,
                Bio = coach.Bio,
                Gender = coach.Gender,
                JoinedDate = coach.JoinedDate,
                AvailableForOnlineTraining = ((Coach)coach).AvailableForOnlineTraining,
                Gym = coach is Coach returnCoach && returnCoach.Gym != null
                    ? new GymResponseDto
                    {
                        GymID = returnCoach.Gym.GymID,
                        GymName = returnCoach.Gym.GymName,
                        Address = returnCoach.Gym.Address,
                        City = returnCoach.Gym.City,
                        PictureUrl = returnCoach.Gym.PictureUrl,
                        Governorate = returnCoach.Gym.Governorate,
                        SubscriptionsCount = returnCoach.Gym.GymSubscriptions?.Count ?? 0,
                        AverageRating = (decimal)(returnCoach.Gym.Ratings != null &&
                                                  returnCoach.Gym.Ratings.Count != 0 ?
                                                  returnCoach.Gym.Ratings.Average(r => r.RatingValue) : 0)
                    }
                    : null,
                OnlineTrainingsGroup = ((Coach)coach).OnlineTrainings != null
                    ? ((Coach)coach).OnlineTrainings?.OfType<OnlineTrainingGroup>()
                    .Select(training => new GetOnlineTrainingGroupDTO
                    {
                        Id = training.Id,
                        Title = training.Title ?? "No Title",
                        Description = training.Description ?? "No Description",
                        Price = training.Price,
                        OfferPrice = training.OfferPrice,
                        NoOfSessionsPerWeek = training.NoOfSessionsPerWeek,
                        DurationOfSession = training.DurationOfSession,
                        OfferEnded = training.OfferEnded,
                        SubscriptionClosed = training.SubscriptionClosed,
                        IsAvailable = training.IsAvailable
                    }).ToList()
                    : null,
                OnlineTrainingsPrivate = ((Coach)coach).OnlineTrainings != null
                    ? ((Coach)coach).OnlineTrainings?.OfType<OnlineTrainingPrivate>()
                    .Select(training => new GetOnlineTrainingPrivateDTO
                    {
                        Id = training.Id,
                        Title = training.Title ?? "No Title",
                        Description = training.Description ?? "No Description",
                        Price = training.Price,
                        OfferPrice = training.OfferPrice,
                        OfferEnded = training.OfferEnded,
                        SubscriptionClosed = training.SubscriptionClosed,
                        IsAvailable = training.IsAvailable
                    }).ToList()
                    : null
            });
            response.IsSuccess = true;
            response.Data = coachDtos;
            return response;
        }

        public async Task<Generalresponse> GetCoachDetailsAsync(string CoachId)
        {
            Generalresponse response = new Generalresponse();

            var user = await repository.GetAsync(e => e.Id == CoachId
                        , includeProperties: "Gym"
           );
            if (user == null)
            {
                response.IsSuccess = true;
                response.Data = "User Not Found.";
                return response;
            }

            var UserDto = new GetCoachDTO
            {
                Id = CoachId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
                Gender = user.Gender,
                JoinedDate = user.JoinedDate,
                AvailableForOnlineTraining = ((Coach)user).AvailableForOnlineTraining,
                Gym = user is Coach returnCoach && returnCoach.Gym != null
                    ? new GymResponseDto
                    {
                        GymID = returnCoach.Gym.GymID,
                        GymName = returnCoach.Gym.GymName,
                        Address = returnCoach.Gym.Address,
                        City = returnCoach.Gym.City,
                        PictureUrl = returnCoach.Gym.PictureUrl,
                        Governorate = returnCoach.Gym.Governorate,
                        SubscriptionsCount = returnCoach.Gym.GymSubscriptions?.Count ?? 0,
                        AverageRating = (decimal)(returnCoach.Gym.Ratings != null &&
                                                  returnCoach.Gym.Ratings.Count != 0 ?
                                                  returnCoach.Gym.Ratings.Average(r => r.RatingValue) : 0)
                    }
                    : null,
                OnlineTrainingsGroup = ((Coach)user).OnlineTrainings != null
                    ? ((Coach)user).OnlineTrainings?.OfType<OnlineTrainingGroup>()
                    .Select(training => new GetOnlineTrainingGroupDTO
                    {
                        Id = training.Id,
                        Title = training.Title ?? "No Title",
                        Description = training.Description ?? "No Description",
                        Price = training.Price,
                        OfferPrice = training.OfferPrice,
                        NoOfSessionsPerWeek = training.NoOfSessionsPerWeek,
                        DurationOfSession = training.DurationOfSession,
                        OfferEnded = training.OfferEnded,
                        SubscriptionClosed = training.SubscriptionClosed,
                        IsAvailable = training.IsAvailable
                    }).ToList()
                    : null,
                OnlineTrainingsPrivate = ((Coach)user).OnlineTrainings != null
                    ? ((Coach)user).OnlineTrainings?.OfType<OnlineTrainingPrivate>()
                    .Select(training => new GetOnlineTrainingPrivateDTO
                    {
                        Id = training.Id,
                        Title = training.Title ?? "No Title",
                        Description = training.Description ?? "No Description",
                        Price = training.Price,
                        OfferPrice = training.OfferPrice,
                        OfferEnded = training.OfferEnded,
                        SubscriptionClosed = training.SubscriptionClosed,
                        IsAvailable = training.IsAvailable
                    }).ToList()
                    : null
            };

            response.IsSuccess = true;
            response.Data = UserDto;
            return response;
        }

        public async Task<Generalresponse> GetTraineeDetailsAsync(string TraineeId)
        {
            Generalresponse response = new Generalresponse();

            var user = await repository.GetAsync(e => e.Id == TraineeId);
            if (user == null)
            {
                response.IsSuccess = true;
                response.Data = "User Not Found.";
                return response;
            }

            var UserDto = new GetTraineeDTO
            {
                Id = TraineeId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
                Gender = user.Gender,
                JoinedDate = user.JoinedDate
            };

            response.IsSuccess = true;
            response.Data = UserDto;
            return response;
        }

        public async Task<Generalresponse> SetOnlineAvailabilityAsync(string userId, bool isAvailable)
        {
            Generalresponse response = new Generalresponse();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException();

            if (user is Coach coach)
            {
                coach.AvailableForOnlineTraining = isAvailable;

                await _userManager.UpdateAsync(coach);

                response.IsSuccess = true;
                response.Data = $"Availability status updated to: {isAvailable}";
                return response;
            }

            response.IsSuccess = false;
            response.Data = "Only coaches can set online availability.";
            return response;
        }

        public async Task<Generalresponse> UpdateProfileDetailsAsync(UpdateProfileDTO profileDTO, string userId)
        {
            Generalresponse response = new Generalresponse();
            var user = await repository.GetAsync(e => e.Id == userId);
            if (user == null)
                throw new UnauthorizedAccessException();

            if (!string.IsNullOrEmpty(profileDTO.FirstName)) user.FirstName = profileDTO.FirstName;
            if (!string.IsNullOrEmpty(profileDTO.LastName)) user.LastName = profileDTO.LastName;
            if (!string.IsNullOrEmpty(profileDTO.Gender)) user.Gender = profileDTO.Gender;
            if (profileDTO.DateOfBirth.HasValue) user.DateOfBirth = profileDTO.DateOfBirth.Value;
            if (!string.IsNullOrEmpty(profileDTO.Bio)) user.Bio = profileDTO.Bio;
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

        public async Task<Generalresponse> ChangeProfilePictureAsync(IFormFile profilePicture, string userId)
        {
            Generalresponse generalresponse = new Generalresponse();
            var user = await repository.GetAsync(e => e.Id == userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (profilePicture == null || profilePicture.Length == 0)
            {
                generalresponse.IsSuccess = false;
                generalresponse.Data = "Invalid File";
                return generalresponse;
            }

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

        public async Task<Generalresponse> DeleteProfilePictureAsync(ApplicationUser user)
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
                generalresponse.Data = "Your Photo has been removed";
                return generalresponse;
            }
            generalresponse.IsSuccess= false;
            generalresponse.Data = "There is no Photo";
            return generalresponse;
        }
        


    }
}
