using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

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
            var gym = await service.GetGymByIdAsync(id);

            if (gym == null) return NotFound("Gym not found");

            return Ok(gym);
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> CreateGym(CreateGymDTO CreateGymDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //if (signInManager.IsSignedIn(User))
            //{
            ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);
            var success = await service.CreateGymAsync(CreateGymDTO, user!);

            //to do: return created gym
            if (success) return Created();

            return BadRequest("Problem creating Gym");
            //}
            //else
            //{
            //    return Unauthorized();
            //}
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateGym(int id, UpdateGymDTO Gym)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await service.UpdateGymAsync(id, Gym);

            if (!success)
                return NotFound("Gym not found");

            return Created();
        }

        [HttpDelete("{id:int}")]
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
