using Core.Entities.OnlineTrainingEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;
using System.Security.Claims;

namespace API.Controllers.OnlineTrainingControllers;


public class OnlineTrainingSubscriptionController(IOnlineTrainingSubscriptionRepository SubscriptionRepo, IOnlineTrainingRepository OnlineTrainingRepo) : BaseApiController
{
    /// <summary>
    /// Retrieves an online training subscription by its unique identifier.
    /// </summary>
    /// <remarks>This method performs an asynchronous operation to fetch the subscription details from the
    /// repository.  If the subscription is found, it is returned as a response DTO. Otherwise, a "Not Found" response
    /// is returned.</remarks>
    /// <param name="id">The unique identifier of the online training subscription to retrieve. Must be a positive integer.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing the online training subscription data if found,  or a <see
    /// cref="NotFoundResult"/> if no subscription exists with the specified identifier.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OnlineTrainingSubscription>> GetOnlineTrainingSubscriptionById(int id)
    {
        OnlineTrainingSubscription? onlineTrainingSubscription = await SubscriptionRepo.GetByIdAsync(id);
        if (onlineTrainingSubscription == null) return NotFound("Online training subscription not found");
        return Ok(onlineTrainingSubscription.ToResponseDto());
    }

    /// <summary>
    /// Retrieves a list of online training subscriptions associated with the currently authenticated trainee.
    /// </summary>
    /// <remarks>This method is accessible only to users with the "Trainee" role. It returns the subscriptions
    /// in a format suitable for client consumption. If no subscriptions are found, a 404 Not Found response is
    /// returned.</remarks>
    /// <returns>An <see cref="ActionResult{T}"/> containing a read-only list of <see cref="OnlineTrainingSubscription"/> objects
    /// representing the trainee's online training subscriptions. Returns a 404 Not Found response if no subscriptions
    /// exist.</returns>
    [HttpGet("ForTrainee")]
    [Authorize(Roles = "Trainee")]
    public async Task<ActionResult<IReadOnlyList<OnlineTrainingSubscription>>> GetOnlineTrainingSubscriptionsForTrainee()
    {
        var traineeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        IReadOnlyList<OnlineTrainingSubscription?> onlineTrainingSubscriptions = await SubscriptionRepo.GetByTraineeIdAsync(traineeId!);
        if (onlineTrainingSubscriptions == null || !onlineTrainingSubscriptions.Any())
            return NotFound("Online training subscription not found");
        // convert list to DTO
        var onlineTrainingSubscriptionDtos = onlineTrainingSubscriptions.Select(x => x!.ToResponseDto()).ToList();
        return Ok(onlineTrainingSubscriptionDtos);
    }
    /// <summary>
    /// Retrieves a list of subscriptions associated with a specific online training session.
    /// </summary>
    /// <remarks>This method is restricted to users with the "Coach" role. The caller must be the owner of the
    /// specified online training session to access its subscriptions. If the caller is not authorized or the online
    /// training session does not exist, appropriate HTTP status codes are returned.</remarks>
    /// <param name="onlineTrainingId">The unique identifier of the online training session.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing a read-only list of <see cref="OnlineTrainingSubscription"/> objects
    /// if the subscriptions are found. Returns <see langword="Unauthorized"/> if the caller is not authorized to access
    /// the subscriptions, or <see langword="NotFound"/> if no subscriptions exist for the specified online training
    /// session.</returns>
    [HttpGet("ByOnlineTrainingId")]
    [Authorize(Roles = "Coach")]
    public async Task<ActionResult<IReadOnlyList<OnlineTrainingSubscription>>> GetOnlineTrainingSubscriptionsByOnlineTrainingId(int onlineTrainingId)
    {
        var CoachId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // Check if the coach is the owner of the online training
        var onlineTraining = await OnlineTrainingRepo.GetByIdAsync(onlineTrainingId);
        if (onlineTraining == null || onlineTraining.CoachID != CoachId)
        {
            return Unauthorized("You are not authorized to access this online training subscriptions");
        }
        IReadOnlyList<OnlineTrainingSubscription?> onlineTrainingSubscriptions = await SubscriptionRepo.GetByOnlineTrainingIdAsync(onlineTrainingId);
        if (onlineTrainingSubscriptions == null || !onlineTrainingSubscriptions.Any())
            return NotFound("Online training subscription not found");
        // convert list to DTO
        var onlineTrainingSubscriptionDtos = onlineTrainingSubscriptions.Select(x => x!.ToResponseDto()).ToList();
        return Ok(onlineTrainingSubscriptionDtos);
    }
}
