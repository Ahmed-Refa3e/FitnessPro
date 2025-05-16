using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Infrastructure.Repositories.GymRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;

namespace API.Controllers.GymAndRating
{
    public class GymRatingController(GymRatingRepository Repo, SignInManager<ApplicationUser> signInManager) : BaseApiController
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

            ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not found");

            var existingRating = await Repo.GetByConditionAsync(x => ((GymRating)x).TraineeID == user.Id && ((GymRating)x).GymID == CreateGymRatingDTO.GymID);

            if (existingRating != null)
                return BadRequest("You have already rated this gym");

            GymRating rating = CreateGymRatingDTO.ToEntity();
            rating.TraineeID = user!.Id;

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

            var user = await signInManager.UserManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized("User is not authenticated");

            var traineeId = user.Id;


            if (await Repo.GetByConditionAsync(x =>
                x is GymRating gr &&
                gr.GymID == GymID &&
                gr.TraineeID == traineeId) is not GymRating gymRatingToBeUpdated)
                return NotFound($"No rating found for GymID {GymID} by this user.");


            gymRatingToBeUpdated.RatingDate = DateTime.Now;
            gymRatingToBeUpdated.RatingValue = UpdateGymRatingDTO.RatingValue;
            gymRatingToBeUpdated.Review = UpdateGymRatingDTO.Review;

            Repo.Update(gymRatingToBeUpdated);

            var saved = await Repo.SaveChangesAsync();
            if (!saved)
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update the gym rating.");

            return Ok(new { IsSuccess = true, Data = "Gym rating updated successfully" });
        }

        [HttpDelete("{GymID:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult> DeleteGymRating(int GymID)
        {
            var user = await signInManager.UserManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized("User is not authenticated");

            var traineeId = user.Id;

            if (await Repo.GetByConditionAsync(x =>
                x is GymRating gr &&
                gr.GymID == GymID &&
                gr.TraineeID == traineeId) is not GymRating gymRatingToBeDeleted)
                return NotFound($"No rating found for GymID {GymID} by this user.");

            Repo.Delete(gymRatingToBeDeleted);

            if (!await Repo.SaveChangesAsync())
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete the gym rating.");

            return Ok(new { IsSuccess = true, Data = "Gym rating deleted successfully" });
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
            ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not found");

            var existingRating = await Repo.GetByConditionAsync(x =>
                ((GymRating)x).TraineeID == user.Id &&
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
            ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not found");

            var userRating = await Repo.GetByConditionAsync(x =>
                ((GymRating)x).TraineeID == user.Id &&
                ((GymRating)x).GymID == gymId);

            if (userRating == null)
                return NotFound("User has not rated this gym");

            return Ok(((GymRating)userRating).ToResponseDto());
        }
    }
}
