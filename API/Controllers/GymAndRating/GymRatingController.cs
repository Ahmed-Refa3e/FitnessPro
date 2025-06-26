using Core.DTOs.GeneralDTO;
using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;
using System.Security.Claims;

namespace API.Controllers.GymAndRating
{
    public class GymRatingController(GymRatingRepository Repo) : BaseApiController
    {
        /// <summary>
        /// Get rating by rating id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GymRatingResponseDTO>> GetGymRatingById(int id)
        {
            GymRating? gymRating = await Repo.GetByIdAsync(id);

            if (gymRating == null) return NotFound("GymRating not found");

            return Ok(gymRating.ToResponseDto());
        }

        [HttpPost]
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> CreateGymRating(CreateGymRatingDTO CreateGymRatingDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var TraineeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingRating = await Repo.GetByConditionAsync(x => ((GymRating)x).TraineeID == TraineeId && ((GymRating)x).GymID == CreateGymRatingDTO.GymID);

            if (existingRating != null)
                return BadRequest("You have already rated this gym");

            GymRating rating = CreateGymRatingDTO.ToEntity();
            rating.TraineeID = TraineeId;

            Repo.Add(rating);

            if (await Repo.SaveChangesAsync())
                return CreatedAtAction(nameof(GetGymRatingById), new { id = rating.GymRatingID }, rating.ToResponseDto());

            return BadRequest("Problem Adding Gym Rating");
        }

        [HttpPut("{GymID:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult> UpdateGymRating(int GymID, UpdateGymRatingDTO UpdateGymRatingDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var traineeId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (await Repo.GetByConditionAsync(x =>
                x is GymRating gr &&
                gr.GymID == GymID &&
                gr.TraineeID == traineeId) is not GymRating gymRatingToBeUpdated)
                return NotFound($"No rating found for GymID {GymID} by this user.");


            gymRatingToBeUpdated.RatingDate = DateTime.Now;
            gymRatingToBeUpdated.RatingValue = UpdateGymRatingDTO.RatingValue;
            gymRatingToBeUpdated.Review = UpdateGymRatingDTO.Review;

            var saved = await Repo.SaveChangesAsync();
            if (!saved)
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update the gym rating.");

            return Ok(new GeneralResponse { IsSuccess = true, Data = "Gym rating updated successfully" });
        }

        [HttpDelete("{GymID:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult> DeleteGymRating(int GymID)
        {

            var traineeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (await Repo.GetByConditionAsync(x =>
                x is GymRating gr &&
                gr.GymID == GymID &&
                gr.TraineeID == traineeId) is not GymRating gymRatingToBeDeleted)
                return NotFound($"No rating found for GymID {GymID} by this user.");

            Repo.Remove(gymRatingToBeDeleted);

            if (!await Repo.SaveChangesAsync())
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete the gym rating.");

            return Ok(new GeneralResponse { IsSuccess = true, Data = "Gym rating deleted successfully" });
        }

        /// <summary>
        /// Endpoint to return if the user has rated the gym or not 
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        [HttpGet("hasRated/{gymId:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult<bool>> HasRatedGym(int gymId)
        {
            var traineeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingRating = await Repo.GetByConditionAsync(x =>
                ((GymRating)x).TraineeID == traineeId &&
                ((GymRating)x).GymID == gymId);

            return Ok(existingRating != null);
        }

        /// <summary>
        /// To get rating for signed in user of the gym by gym id
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        [HttpGet("UserRating/{gymId:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult<GymRatingResponseDTO>> GetUserGymRating(int gymId)
        {
            var traineeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userRating = await Repo.GetByConditionAsync(x =>
                ((GymRating)x).TraineeID == traineeId &&
                ((GymRating)x).GymID == gymId);

            if (userRating == null)
                return NotFound("User has not rated this gym");

            return Ok(((GymRating)userRating).ToResponseDto());
        }
    }
}
