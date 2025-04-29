using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;

namespace API.Controllers.GymAndRating
{
    public class GymController(IGymService service, SignInManager<ApplicationUser> signInManager) : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Gym>>> GetGyms([FromQuery] GetGymDTO GetGymDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pagedResult = await service.GetGymsAsync(GetGymDTO);

            return Ok(pagedResult);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Gym>> GetGymById(int id)
        {
            Gym? gym = await service.GetGymByIdAsync(id);

            if (gym == null) return NotFound("Gym not found");
            //convert to response dto using extension method
            GymResponseDetailsDto gymResponse = gym.ToResponseDetailsDto();

            return Ok(gymResponse);
        }

        [HttpGet("ByCoachId")]
        public async Task<ActionResult<Gym>> GetGymByCoachId(string CoachId)
        {
            Gym? gym = await service.GetGymByCoachIdAsync(CoachId);
            if (gym == null) return NotFound("Gym not found");
            //convert to response dto using extension method
            GymResponseDetailsDto gymResponse = gym.ToResponseDetailsDto();

            return Ok(gymResponse);
        }

        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> CreateGym(CreateGymDTO CreateGymDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);

            var success = await service.CreateGymAsync(CreateGymDTO, user!);

            if (success)
                return Created(string.Empty, new { IsSuccess = true, Data = "Gym created successfully" });

            return BadRequest("Problem creating Gym");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> UpdateGym(int id, UpdateGymDTO Gym)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await signInManager.UserManager.GetUserAsync(User);
            var gymToBeUpdated = await service.GetGymByIdAsync(id); // Await the task to get the actual Gym object

            // Check if gym is found
            if (gymToBeUpdated == null)
                return NotFound("Gym not found");

            if (gymToBeUpdated.CoachID != user!.Id)
            {
                return Unauthorized("You are not authorized to update this gym");
            }

            var success = await service.UpdateGymAsync(id, Gym);

            if (!success)
                return NotFound("Problem updating Gym");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> DeleteGym(int id)
        {
            var user = await signInManager.UserManager.GetUserAsync(User);
            var gymToBeDeleted = service.GetGymByIdAsync(id);
            // Check if gym is found
            if (gymToBeDeleted == null)
                return NotFound("Gym not found");
            if (gymToBeDeleted.Result!.CoachID != user!.Id)
            {
                return Unauthorized("You are not authorized to Delete this gym");
            }

            var success = await service.DeleteGymAsync(id);

            if (!success) return NotFound("Problem deleting Gym");

            return NoContent();
        }

        [HttpGet("Cities")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCities()
        {
            return Ok(await service.GetCitiesAsync());
        }
    }
}
