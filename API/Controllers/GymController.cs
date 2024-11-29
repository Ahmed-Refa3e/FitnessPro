using Core.DTOs;
using Core.Entities.GymEntities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class GymController(IGymRepository repo) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Gym>>> GetGyms([FromQuery] GetGymDTO GymDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var pagedResult = await repo.GetGymsAsync(
            GymDTO.City,
            GymDTO.Governorate,  // Filter by Governorate
            GymDTO.GymName,      // GymName for searching
            GymDTO.PageNumber,
            GymDTO.PageSize,
            GymDTO.SortBy        // Sort by "subscriptions", "rating", "highestPrice", or "lowestPrice"
        );

        return Ok(pagedResult);
    }

    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Gym>> GetGymById(int id)
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
            return CreatedAtAction("CreateGym", new { id = Gym.GymID }, Gym);
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