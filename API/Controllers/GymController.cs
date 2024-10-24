using API.DTOs;
using Core.Entities.GymEntities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class GymController(IGymRepository repo) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Gym>>> GetGyms([FromQuery]GymDTO GymDTO)
    {

        var pagedResult = await repo.GetGymsAsync(GymDTO.City, GymDTO.PageNumber, GymDTO.PageSize);

        return Ok(pagedResult);
    }

    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Gym>> GetGym(int id)
    {
        var product = await repo.GetGymByIdAsync(id);

        if (product == null) return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Gym>> CreateGym(Gym Gym)
    {
        repo.AddGym(Gym);

        if (await repo.SaveChangesAsync())
        {
            return CreatedAtAction("GetGym", new { id = Gym.GymID }, Gym);
        }

        return BadRequest("Problem creating Gym");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateGym(int id, Gym Gym)
    {
        if (Gym.GymID != id || !GymExists(id))
            return BadRequest("Cannot update this Gym");

        repo.UpdateGym(Gym);

        if (await repo.SaveChangesAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem updating the Gym");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteGym(int id)
    {
        var Gym = await repo.GetGymByIdAsync(id);

        if (Gym == null) return NotFound();

        repo.DeleteGym(Gym);

        if (await repo.SaveChangesAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem deleting the Gym");
    }

    [HttpGet("Cities")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCities()
    {
        return Ok(await repo.GetCitiesAsync());
    }

    private bool GymExists(int id)
    {
        return repo.GymExists(id);
    }
}