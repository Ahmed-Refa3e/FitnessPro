using Core.DTOs.GeneralDTO;
using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;
using System.Security.Claims;

namespace API.Controllers.GymAndRating
{
    public class GymController(IGymService service, SignInManager<ApplicationUser> signInManager) : BaseApiController
    {
        /// <summary>
        /// Retrieves a paginated list of gyms based on the specified query parameters.
        /// </summary>
        /// <remarks>This method uses the <c>[HttpGet]</c> attribute to handle HTTP GET requests.  Ensure
        /// that the query parameters provided in <paramref name="GetGymDTO"/> are valid to avoid a bad request
        /// response.</remarks>
        /// <param name="GetGymDTO">The query parameters used to filter and paginate the list of gyms.  This must include valid data as per the
        /// model requirements.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing a read-only list of gyms that match the query parameters.
        /// Returns a <see cref="BadRequestResult"/> if the model state is invalid.</returns>

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

            var CoachId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var gymToBeUpdated = await service.GetGymByIdAsync(id); // Await the task to get the actual Gym object

            // Check if gym is found
            if (gymToBeUpdated == null)
                return NotFound("Gym not found");

            if (gymToBeUpdated.CoachID != CoachId)
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
            var CoachId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var gymToBeDeleted = service.GetGymByIdAsync(id);
            // Check if gym is found
            if (gymToBeDeleted == null)
                return NotFound("Gym not found");
            if (gymToBeDeleted.Result!.CoachID != CoachId)
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
        /// <summary>
        /// Retrieves a list of nearby gyms based on the specified search criteria.
        /// </summary>
        /// <remarks>This method uses the provided search parameters to query for gyms within the
        /// specified radius. Ensure that the <paramref name="dto"/> contains valid data, as invalid input will result
        /// in a <see cref="BadRequestResult"/>.</remarks>
        /// <param name="dto">The data transfer object containing the search parameters, such as location and radius.</param>
        /// <returns>An <see cref="ActionResult"/> containing a list of gyms that match the search criteria. If no gyms are
        /// found, returns a <see cref="NotFoundResult"/> with an appropriate message. If the request is invalid,
        /// returns a <see cref="BadRequestResult"/> with validation errors.</returns>
        [HttpGet("Nearby")]
        public async Task<ActionResult<List<GymResponseDto>>> GetNearbyGyms([FromQuery] GetNearbyGymsDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<GymResponseDto> gyms = await service.GetNearbyGymsAsync(dto);
            if (gyms.Count == 0)
                return NotFound("No nearby gyms found");
            return Ok(new GeneralResponse {IsSuccess =true,Data = gyms});
        }
        /// <summary>
        /// Adds a new location to the specified gym.
        /// </summary>
        /// <remarks>This action requires the user to be authenticated and authorized with the "Coach"
        /// role. Ensure that the <paramref name="dto"/> contains valid latitude and longitude values.</remarks>
        /// <param name="id">The unique identifier of the gym to which the location will be added.</param>
        /// <param name="dto">An object containing the latitude and longitude of the new location. The <see cref="UpdateGymLocationDTO"/>
        /// must include valid geographic coordinates.</param>
        /// <returns>Returns a <see cref="NoContentResult"/> if the location is successfully added. Returns a <see
        /// cref="NotFoundResult"/> if the specified gym does not exist. Returns an <see cref="UnauthorizedResult"/> if
        /// the current user is not authorized to modify the gym. Returns a <see cref="BadRequestResult"/> if the
        /// location could not be added or already exists.</returns>
        [HttpPut("{id:int}/AddLocation")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> AddLocation(int id, [FromBody] UpdateGymLocationDTO dto)
        {
            string? CoachId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var gym = await service.GetGymByIdAsync(id);

            if (gym == null) return NotFound("Gym not found");
            if (gym.CoachID != CoachId) return Unauthorized();

            var result = await service.AddGymLocationAsync(id, dto.Latitude, dto.Longitude);
            return result ? NoContent() : BadRequest("Add location failed or already exists");
        }
        /// <summary>
        /// Updates the location of a gym specified by its ID.
        /// </summary>
        /// <remarks>This endpoint is restricted to users with the "Coach" role. The authenticated user
        /// must be the coach associated with the gym to perform the update.</remarks>
        /// <param name="id">The unique identifier of the gym to update.</param>
        /// <param name="dto">An object containing the new latitude and longitude values for the gym's location.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns: <list type="bullet">
        /// <item><description><see cref="NotFound"/> if the gym with the specified ID does not
        /// exist.</description></item> <item><description><see cref="UnauthorizedResult"/> if the current user is not
        /// authorized to update the gym's location.</description></item> <item><description><see cref="NoContent"/> if
        /// the location update is successful.</description></item> <item><description><see cref="BadRequest"/> if the
        /// update operation fails.</description></item> </list></returns>

        [HttpPut("{id:int}/UpdateLocation")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateGymLocationDTO dto)
        {
            var CoachId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var gym = await service.GetGymByIdAsync(id);

            if (gym == null) return NotFound("Gym not found");
            if (gym.CoachID != CoachId) return Unauthorized();

            var result = await service.UpdateGymLocationAsync(id, dto.Latitude, dto.Longitude);
            return result ? NoContent() : BadRequest("Update failed");
        }
    }
}
