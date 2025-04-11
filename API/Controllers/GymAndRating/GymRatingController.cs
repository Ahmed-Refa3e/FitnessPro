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
        public async Task<ActionResult<GymRating>> GetGymRatingById(int id)
        {
            GymRating? gymRating = await Repo.GetByIdAsync(id);

            if (gymRating == null) return NotFound("GymRating not found");

            return Ok(gymRating);
        }

        [HttpPost]
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> CreateGymRating(CreateGymRatingDTO CreateGymRatingDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);

            GymRating Rating = CreateGymRatingDTO.ToEntity();
            Rating.TraineeID = user!.Id;

            Repo.Add(Rating);

            if (await Repo.SaveChangesAsync()) return Created();

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
            if (gymRatingToBeUpdated!.TraineeID != user!.Id)
            {
                return Unauthorized("You are not authorized to update this gym rating");
            }

            gymRatingToBeUpdated.RatingDate = DateTime.Now;
            gymRatingToBeUpdated.RatingValue = UpdateGymRatingDTO.RatingValue;
            gymRatingToBeUpdated.Review = UpdateGymRatingDTO.Review;

            Repo.Update(gymRatingToBeUpdated);

            if (!await Repo.SaveChangesAsync())
                return NotFound("Gym rating not found");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult> DeleteGym(int id)
        {
            var gymRatingToBeDeleted = await Repo.GetByIdAsync(id);
            var user = await signInManager.UserManager.GetUserAsync(User);
            if (gymRatingToBeDeleted!.TraineeID != user!.Id)
            {
                return Unauthorized("You are not authorized to Delete this gym rating");
            }

            Repo.Delete(gymRatingToBeDeleted);

            if (!await Repo.SaveChangesAsync())
                return NotFound("Gym rating not found");

            return NoContent();
        }
    }
}
