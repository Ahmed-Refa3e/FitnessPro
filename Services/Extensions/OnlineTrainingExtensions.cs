using Core.DTOs.OnlineTrainingDTO;
using Core.DTOs.OnlineTrainingSubscriptionDTO;
using Core.Entities.OnlineTrainingEntities;

namespace Services.Extensions
{
    public static class OnlineTrainingExtensions
    {
        public static OnlineTrainingResponseDto ToResponseDto(this OnlineTraining onlineTraining)
        {
            return onlineTraining == null
                ? throw new ArgumentNullException(nameof(onlineTraining))
                : new OnlineTrainingResponseDto
                {
                    Id = onlineTraining.Id,
                    CoachID = onlineTraining.CoachID,
                    Title = onlineTraining.Title,
                    Description = onlineTraining.Description,
                    TrainingType = onlineTraining.TrainingType,
                    Price = onlineTraining.Price,
                    NoOfSessionsPerWeek = onlineTraining.NoOfSessionsPerWeek,
                    DurationOfSession = onlineTraining.DurationOfSession
                };
        }
        public static OnlineTrainingSubscriptionResponseDto ToResponseDto(this OnlineTrainingSubscription onlineTrainingSubscription)
        {
            return onlineTrainingSubscription == null
                ? throw new ArgumentNullException(nameof(onlineTrainingSubscription))
                : new OnlineTrainingSubscriptionResponseDto
                {
                    Id = onlineTrainingSubscription.Id,
                    TraineeID = onlineTrainingSubscription.TraineeID!,
                    OnlineTrainingId = onlineTrainingSubscription.OnlineTrainingId,
                    StartDate = onlineTrainingSubscription.StartDate,
                    EndDate = onlineTrainingSubscription.EndDate,
                    OnlineTrainingTitle = onlineTrainingSubscription.OnlineTraining?.Title,
                    TraineeName = onlineTrainingSubscription.Trainee?.FirstName + onlineTrainingSubscription.Trainee?.LastName,
                    IsActive = onlineTrainingSubscription.IsActive
                };
        }
    }
}
