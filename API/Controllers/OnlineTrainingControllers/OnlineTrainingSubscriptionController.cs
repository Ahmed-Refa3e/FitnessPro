using Core.Entities.OnlineTrainingEntities;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;

namespace API.Controllers.OnlineTrainingControllers
{
    public class OnlineTrainingSubscriptionController(IOnlineTrainingSubscriptionRepository repo) : BaseApiController
    {
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OnlineTrainingSubscription>> GetOnlineTrainingSubscriptionById(int id)
        {
            OnlineTrainingSubscription? onlineTrainingSubscription = await repo.GetByIdAsync(id);
            if (onlineTrainingSubscription == null) return NotFound("Online training subscription not found");
            return Ok(onlineTrainingSubscription.ToResponseDto());
        }
        [HttpGet("ByTraineeId")]
        public async Task<ActionResult<IReadOnlyList<OnlineTrainingSubscription>>> GetOnlineTrainingSubscriptionByTraineeId(string traineeId)
        {
            IReadOnlyList<OnlineTrainingSubscription?> onlineTrainingSubscriptions = await repo.GetByTraineeIdAsync(traineeId);
            if (onlineTrainingSubscriptions == null || !onlineTrainingSubscriptions.Any())
                return NotFound("Online training subscription not found");
            // convert list to DTO
            var onlineTrainingSubscriptionDtos = onlineTrainingSubscriptions.Select(x => x!.ToResponseDto()).ToList();
            return Ok(onlineTrainingSubscriptionDtos);
        }
        [HttpGet("ByOnlineTrainingId")]
        public async Task<ActionResult<IReadOnlyList<OnlineTrainingSubscription>>> GetOnlineTrainingSubscriptionByOnlineTrainingId(int onlineTrainingId)
        {
            IReadOnlyList<OnlineTrainingSubscription?> onlineTrainingSubscriptions = await repo.GetByOnlineTrainingIdAsync(onlineTrainingId);
            if (onlineTrainingSubscriptions == null || !onlineTrainingSubscriptions.Any())
                return NotFound("Online training subscription not found");
            // convert list to DTO
            var onlineTrainingSubscriptionDtos = onlineTrainingSubscriptions.Select(x => x!.ToResponseDto()).ToList();
            return Ok(onlineTrainingSubscriptionDtos);
        }
    }
}
