using Core.DTOs.OnlineTrainingDTO;
using Core.Entities.OnlineTrainingEntities;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;

namespace API.Controllers.OnlineTrainingControllers
{
    public class OnlineTrainingController(IOnlineTrainingRepository Repo, SignInManager<ApplicationUser> signInManager) : BaseApiController
    {

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OnlineTraining>> GetOnlineTrainingById(int id)
        {
            OnlineTraining? OnlineTraining = await Repo.GetByIdAsync(id);

            if (OnlineTraining == null) return NotFound("Online training not found");

            return Ok(OnlineTraining.ToResponseDto());
        }

        [HttpGet("ByCoachId/Group")]
        public async Task<ActionResult<IReadOnlyList<OnlineTraining>>> GetGroupOnlineTrainingByCoachId(string CoachId)
        {
            IReadOnlyList<OnlineTraining?> OnlineTrainings = await Repo.GetGroupTrainingByCoachIdAsync(CoachId);
            if (OnlineTrainings == null || !OnlineTrainings.Any()) return NotFound("Group Online training not found");
            //convert to DTO
            var OnlineTrainingDtos = OnlineTrainings.Select(onlineTraining => onlineTraining!.ToResponseDto()).ToList();
            return Ok(OnlineTrainingDtos);
        }

        [HttpGet("ByCoachId/Private")]
        public async Task<ActionResult<IReadOnlyList<OnlineTraining>>> GetPrivateOnlineTrainingByCoachId(string CoachId)
        {
            IReadOnlyList<OnlineTraining?> OnlineTrainings = await Repo.GetPrivateTrainingByCoachIdAsync(CoachId);
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

            ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);

            var onlineTraining = new OnlineTraining
            {
                CoachID = user!.Id,
                Title = createOnlineTrainingDTO.Title,
                Description = createOnlineTrainingDTO.Description,
                TrainingType = createOnlineTrainingDTO.TrainingType,
                Price = createOnlineTrainingDTO.Price,
                NoOfSessionsPerWeek = createOnlineTrainingDTO.NoOfSessionsPerWeek,
                DurationOfSession = createOnlineTrainingDTO.DurationOfSession
            };

            Repo.Add(onlineTraining);
            if (await Repo.SaveChangesAsync())
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

            var onlineTrainingToBeUpdated = await Repo.GetByIdAsync(id);
            if (onlineTrainingToBeUpdated == null)
                return NotFound("Online training not found");

            var user = await signInManager.UserManager.GetUserAsync(User);
            if (onlineTrainingToBeUpdated.CoachID != user!.Id)
            {
                return Unauthorized("You are not authorized to update this Online training");
            }

            onlineTrainingToBeUpdated.Title = createOnlineTrainingDTO.Title;
            onlineTrainingToBeUpdated.Description = createOnlineTrainingDTO.Description;
            onlineTrainingToBeUpdated.TrainingType = createOnlineTrainingDTO.TrainingType;
            onlineTrainingToBeUpdated.Price = createOnlineTrainingDTO.Price;
            onlineTrainingToBeUpdated.NoOfSessionsPerWeek = createOnlineTrainingDTO.NoOfSessionsPerWeek;
            onlineTrainingToBeUpdated.DurationOfSession = createOnlineTrainingDTO.DurationOfSession;

            Repo.Update(onlineTrainingToBeUpdated);
            var success = await Repo.SaveChangesAsync();

            if (!success)
                return NotFound("Online training not found");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> DeleteOnlineTraining(int id)
        {
            var OnlineTrainingToBeDeleted = await Repo.GetByIdAsync(id);
            if (OnlineTrainingToBeDeleted == null) return NotFound("Online training not found");
            var user = await signInManager.UserManager.GetUserAsync(User);
            if (OnlineTrainingToBeDeleted!.CoachID != user!.Id)
            {
                return Unauthorized("You are not authorized to Delete this online training");
            }

            Repo.Delete(OnlineTrainingToBeDeleted);

            if (!await Repo.SaveChangesAsync()) return NotFound("Online training not found");

            return NoContent();
        }
    }
}
