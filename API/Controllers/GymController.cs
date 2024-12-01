using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class GymController(IGymService service) : BaseApiController
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

            if (gym == null) return NotFound();

            return gym;
        }

        [HttpPost]
        public async Task<ActionResult<Gym>> CreateGym(Gym Gym)
        {
            var success = await service.CreateGymAsync(Gym);

            if (success)
                return CreatedAtAction("CreateGym", new { id = Gym.GymID }, Gym);

            return BadRequest("Problem creating Gym");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateGym(int id, Gym Gym)
        {
            var success = await service.UpdateGymAsync(id, Gym);

            if (!success)
                return BadRequest("Cannot update this Gym");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteGym(int id)
        {
            var success = await service.DeleteGymAsync(id);

            if (!success) return NotFound();

            return NoContent();
        }

        [HttpGet("Cities")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCities()
        {
            return Ok(await service.GetCitiesAsync());
        }
    }
}
