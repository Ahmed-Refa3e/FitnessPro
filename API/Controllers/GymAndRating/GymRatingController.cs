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

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult> UpdateGymRating(int id, UpdateGymRatingDTO UpdateGymRatingDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var gymRatingToBeUpdated = await Repo.GetByIdAsync(id);
            var user = await signInManager.UserManager.GetUserAsync(User);
            if (gymRatingToBeUpdated == null)
                return NotFound("Gym rating not found");

            if (gymRatingToBeUpdated.TraineeID != user!.Id)
                return Unauthorized("You are not authorized to update this gym rating");

            gymRatingToBeUpdated.RatingDate = DateTime.Now;
            gymRatingToBeUpdated.RatingValue = UpdateGymRatingDTO.RatingValue;
            gymRatingToBeUpdated.Review = UpdateGymRatingDTO.Review;

            Repo.Update(gymRatingToBeUpdated);

            if (!await Repo.SaveChangesAsync())
                return BadRequest("Problem updating gym rating");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult> DeleteGymRating(int id)
        {
            var gymRatingToBeDeleted = await Repo.GetByIdAsync(id);
            var user = await signInManager.UserManager.GetUserAsync(User);
            if (gymRatingToBeDeleted == null)
                return NotFound("Gym rating not found");

            if (gymRatingToBeDeleted.TraineeID != user!.Id)
                return Unauthorized("You are not authorized to Delete this gym rating");

            Repo.Delete(gymRatingToBeDeleted);

            if (!await Repo.SaveChangesAsync())
                return BadRequest("Problem deleting gym rating");

            return NoContent();
        }

        // Endpoint to return if the user has rated the gym or not 
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

        // Endpoint to return the rating of the user for the gym
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
