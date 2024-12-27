using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;

namespace API.Controllers
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
        [Authorize(Roles ="Coach")]
        public async Task<IActionResult> CreateGym(CreateGymDTO CreateGymDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

                ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);
                var success = await service.CreateGymAsync(CreateGymDTO, user!);

            if (success) return Created();
                   // CreatedAtAction(nameof(GetGymById), CreateGymDTO);

                return BadRequest("Problem creating Gym");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles ="Coach")]

        //ToDo th update method should be updated to allow only the coach who created the gym to update it
        public async Task<ActionResult> UpdateGym(int id, UpdateGymDTO Gym)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await service.UpdateGymAsync(id, Gym);

            if (!success)
                return NotFound("Gym not found");

            return NoContent();
        }

        //ToDo th delete method should be updated to allow only the coach who created the gym to delete it
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> DeleteGym(int id)
        {
            var success = await service.DeleteGymAsync(id);

            if (!success) return NotFound("Gym not found");

            return NoContent();
        }

        [HttpGet("Cities")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCities()
        {
            return Ok(await service.GetCitiesAsync());
        } 
    }
}
