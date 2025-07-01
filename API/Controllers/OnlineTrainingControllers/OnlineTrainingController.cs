using Core.DTOs.OnlineTrainingDTO;
using Core.Entities.OnlineTrainingEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;
using System.Security.Claims;

namespace API.Controllers.OnlineTrainingControllers
{
    public class OnlineTrainingController(IUnitOfWork unitOfWork) : BaseApiController
    {

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OnlineTraining>> GetOnlineTrainingById(int id)
        {
            OnlineTraining? OnlineTraining = await unitOfWork.OnlineTrainingRepository.GetByIdAsync(id);

            if (OnlineTraining == null) return NotFound("Online training not found");

            return Ok(OnlineTraining.ToResponseDto());
        }

        [HttpGet("ByCoachId/Group")]
        public async Task<ActionResult<IReadOnlyList<OnlineTraining>>> GetGroupOnlineTrainingByCoachId(string CoachId)
        {
            IReadOnlyList<OnlineTraining?> OnlineTrainings = await unitOfWork.OnlineTrainingRepository.GetGroupTrainingByCoachIdAsync(CoachId);
            if (OnlineTrainings == null || !OnlineTrainings.Any()) return NotFound("Group Online training not found");
            //convert to DTO
            var OnlineTrainingDtos = OnlineTrainings.Select(onlineTraining => onlineTraining!.ToResponseDto()).ToList();
            return Ok(OnlineTrainingDtos);
        }

        [HttpGet("ByCoachId/Private")]
        public async Task<ActionResult<IReadOnlyList<OnlineTraining>>> GetPrivateOnlineTrainingByCoachId(string CoachId)
        {
            IReadOnlyList<OnlineTraining?> OnlineTrainings = await unitOfWork.OnlineTrainingRepository.GetPrivateTrainingByCoachIdAsync(CoachId);
            if (OnlineTrainings == null || !OnlineTrainings.Any()) return NotFound("Private Online training not found");
            var OnlineTrainingDtos = OnlineTrainings.Select(onlineTraining => onlineTraining!.ToResponseDto()).ToList();
            return Ok(OnlineTrainingDtos);
        }

        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> CreateOnlineTraining(CreateOnlineTrainingDTO createOnlineTrainingDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string CoachId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var onlineTraining = new OnlineTraining
            {
                CoachID = CoachId,
                Title = createOnlineTrainingDTO.Title,
                Description = createOnlineTrainingDTO.Description,
                TrainingType = createOnlineTrainingDTO.TrainingType,
                Price = createOnlineTrainingDTO.Price,
                NoOfSessionsPerWeek = createOnlineTrainingDTO.NoOfSessionsPerWeek,
                DurationOfSession = createOnlineTrainingDTO.DurationOfSession
            };

            unitOfWork.OnlineTrainingRepository.Add(onlineTraining);
            if (await unitOfWork.CompleteAsync())
            {
                return CreatedAtAction(nameof(GetOnlineTrainingById), new { id = onlineTraining.Id }, onlineTraining);
            }

            return BadRequest("Problem creating Online training");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> UpdateOnlineTraining(int id, CreateOnlineTrainingDTO createOnlineTrainingDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var onlineTrainingToBeUpdated = await unitOfWork.OnlineTrainingRepository.GetByIdAsync(id);
            if (onlineTrainingToBeUpdated == null)
                return NotFound("Online training not found");

            string CoachId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (onlineTrainingToBeUpdated.CoachID != CoachId)
            {
                return Unauthorized("You are not authorized to update this Online training");
            }

            onlineTrainingToBeUpdated.Title = createOnlineTrainingDTO.Title;
            onlineTrainingToBeUpdated.Description = createOnlineTrainingDTO.Description;
            onlineTrainingToBeUpdated.TrainingType = createOnlineTrainingDTO.TrainingType;
            onlineTrainingToBeUpdated.Price = createOnlineTrainingDTO.Price;
            onlineTrainingToBeUpdated.NoOfSessionsPerWeek = createOnlineTrainingDTO.NoOfSessionsPerWeek;
            onlineTrainingToBeUpdated.DurationOfSession = createOnlineTrainingDTO.DurationOfSession;

            unitOfWork.OnlineTrainingRepository.Update(onlineTrainingToBeUpdated);
            var success = await unitOfWork.CompleteAsync();

            if (!success)
                return NotFound("Online training not found");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> DeleteOnlineTraining(int id)
        {
            var training = await unitOfWork.OnlineTrainingRepository.GetByIdAsync(id);
            if (training is null)
                return NotFound("Online training not found.");

            var coachId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(coachId) || training.CoachID != coachId)
                return Forbid("You are not authorized to delete this online training.");

            unitOfWork.OnlineTrainingRepository.Remove(training);

            var success = await unitOfWork.CompleteAsync();
            if (!success)
                return StatusCode(500, "An error occurred while deleting the training.");

            return NoContent();
        }

    }
}
