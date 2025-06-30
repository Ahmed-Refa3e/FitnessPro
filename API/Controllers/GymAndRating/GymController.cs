using Core.DTOs.GeneralDTO;
using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
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

            var gymResponse = gym.ToResponseDetailsDto();
            return Ok(gymResponse);
        }

        [HttpGet("ByCoachId")]
        public async Task<ActionResult<Gym>> GetGymByCoachId(string CoachId)
        {
            var gym = await service.GetGymByCoachIdAsync(CoachId);
            if (gym == null) return NotFound("Gym not found");

            var gymResponse = gym.ToResponseDetailsDto();
            return Ok(gymResponse);
        }

        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> CreateGym(CreateGymDTO CreateGymDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await signInManager.UserManager.GetUserAsync(User);
            var success = await service.CreateGymAsync(CreateGymDTO, user!);

            return success
                ? Created(string.Empty, new { IsSuccess = true, Data = "Gym created successfully" })
                : BadRequest("Problem creating Gym");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> UpdateGym(int id, UpdateGymDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authResult = await GetAuthorizedGym(id);
            if (authResult.Result != null) return authResult.Result;

            var success = await service.UpdateGymAsync(id, dto);
            return success ? NoContent() : BadRequest("Problem updating Gym");
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> DeleteGym(int id)
        {
            var authResult = await GetAuthorizedGym(id);
            if (authResult.Result is not null) return authResult.Result;

            var success = await service.DeleteGymAsync(id);
            return success ? NoContent() : BadRequest("Problem deleting Gym");
        }

        [HttpGet("Cities")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCities()
        {
            return Ok(await service.GetCitiesAsync());
        }

        [HttpGet("Nearby")]
        public async Task<ActionResult<List<GymResponseDto>>> GetNearbyGyms([FromQuery] GetNearbyGymsDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var gyms = await service.GetNearbyGymsAsync(dto);
            return gyms.Count == 0
                ? NotFound("No nearby gyms found")
                : Ok(new GeneralResponse { IsSuccess = true, Data = gyms });
        }

        [HttpPut("{id:int}/AddLocation")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> AddLocation(int id, [FromBody] UpdateGymLocationDTO dto)
        {
            var authResult = await GetAuthorizedGym(id);
            if (authResult.Result is not null) return authResult.Result;

            var result = await service.AddGymLocationAsync(id, dto.Latitude, dto.Longitude);
            return result ? NoContent() : BadRequest("Add location failed or already exists");
        }

        [HttpPut("{id:int}/UpdateLocation")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateGymLocationDTO dto)
        {
            var authResult = await GetAuthorizedGym(id);
            if (authResult.Result is not null) return authResult.Result;

            var result = await service.UpdateGymLocationAsync(id, dto.Latitude, dto.Longitude);
            return result ? NoContent() : BadRequest("Update failed");
        }

        /// <summary>
        /// Helper method to check gym ownership and existence.
        /// Returns ActionResult with error if unauthorized or not found.
        /// Otherwise returns the Gym object.
        /// </summary>
        private async Task<ActionResult<Gym>> GetAuthorizedGym(int id)
        {
            var coachId = signInManager.UserManager.GetUserId(User);
            var gym = await service.GetGymByIdAsync(id);

            if (gym is null)
                return NotFound("Gym not found");

            if (gym.CoachID != coachId)
                return Unauthorized("You are not authorized");

            return gym;
        }
    }
}
