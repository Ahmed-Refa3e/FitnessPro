using Core.DTOs.OnlineTrainingDTO;
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
    }
}
