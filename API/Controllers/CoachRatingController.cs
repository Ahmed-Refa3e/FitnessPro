using Core.DTOs.UserDTO;
using Core.Entities.OnlineTrainingEntities;
using Infrastructure.Repositories.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoachRatingController(UserManager<ApplicationUser> userManager, CoachRatingRepository repository) : BaseApiController
    {
        [HttpPost]
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> CreateCoachRating(CreateCoachRatingDTO coachRatingDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User Not Found");

            var ratingFromDb = repository.GetQueryable()
                .Where(e => e.TraineeId == user.Id && e.CoachId == coachRatingDTO.coachId)
                .FirstOrDefault();
            if (ratingFromDb != null)
                return Conflict($"You already add rating to this Coach");

            var coachRating = new CoachRating()
            {
                TraineeId = user.Id,
                CoachId = coachRatingDTO.coachId,
                Content = coachRatingDTO.Review,
                CreatedAt = DateTime.UtcNow,
                Rating = coachRatingDTO.ratingValue
            };
            repository.Add(coachRating);

            var saved = await repository.SaveChangesAsync();
            if (!saved)
                return BadRequest("There is a problem while Adding Rating");

            var response = new CoachRatingResponse
            {
                CoachRatingId = coachRating.CoachRatingId,
                CoachId = coachRating.CoachId,
                TraineeId = coachRating.TraineeId,
                Review = coachRating.Content,
                ratingValue = coachRating.Rating,
                CreatedAt = coachRating.CreatedAt
            };
            return CreatedAtAction(nameof(GetCoachRatingById), new { id = coachRating.CoachRatingId }, response);
        }


        [HttpGet("GetCoachRatingByCoachRatingId/{id}")]
        public async Task<IActionResult> GetCoachRatingById(int id)
        {
            var coachRating = await repository.GetByIdAsync(id);
            if (coachRating == null)
                return NotFound($"Coach Rating with ID {id} not found");

            var response = new CoachRatingResponse()
            {
                CoachId = coachRating.CoachId,
                CoachRatingId = coachRating.CoachRatingId,
                TraineeId = coachRating.TraineeId,
                ratingValue = coachRating.Rating,
                Review = coachRating.Content,
                CreatedAt = coachRating.CreatedAt
            };
            return Ok(response);
        }

        [Authorize(Roles = "Trainee")]
        [HttpGet("GetCoachRatingByCoachId/{coachId}")]
        public async Task<IActionResult> GetCoachRatingByCoachId(string coachId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User Not Found");


            var coachRating = repository.GetQueryable()
                .Where(e => e.CoachId == coachId && e.TraineeId == user.Id)
                .FirstOrDefault();

            if (coachRating == null)
                return NotFound("You didn't add any rating");

            var response = new CoachRatingResponse()
            {
                CoachId = coachRating.CoachId,
                CoachRatingId = coachRating.CoachRatingId,
                TraineeId = coachRating.TraineeId,
                ratingValue = coachRating.Rating,
                Review = coachRating.Content,
                CreatedAt = coachRating.CreatedAt
            };
            return Ok(response);
        }

        [HttpGet("GetAllCoachRating/{CoachId}")]
        public async Task<IActionResult> GetAllCoachRating(string CoachId)
        {
            if (string.IsNullOrWhiteSpace(CoachId))
                return BadRequest("Coach ID is required");

            var query = repository.GetQueryable().Where(e => e.CoachId == CoachId);

            var rating = await query.Select(e => new
            {
                e.CoachRatingId,
                e.Rating,
                e.TraineeId,
                e.CoachId,
                e.CreatedAt,
                e.Content
            }).ToListAsync();

            if (!rating.Any())
                return NotFound("No ratings found");

            return Ok(rating);
        }

        [Authorize(Roles = "Trainee")]
        [HttpPut(("{coachId}"))]
        public async Task<IActionResult> UpdateCoachRating(string coachId, UpdateCoachRatingDTO coachRatingDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User Not Found");


            var coachRating = repository.GetQueryable()
                .Where(e=>e.CoachId == coachId && e.TraineeId == user.Id)
                .FirstOrDefault();

            if (coachRating == null)
                return NotFound("You didn't add any rating");

            if (user?.Id != coachRating.TraineeId)
                return Unauthorized("You can't update this rating");

            coachRating.Rating = coachRatingDTO.RatingValue;
            coachRating.Content = coachRatingDTO.Content;
            coachRating.CreatedAt = DateTime.Now;

            repository.Update(coachRating);
            if (!await repository.SaveChangesAsync())
                return BadRequest("Error updating rating");
            return NoContent();
        }

        [HttpDelete("{coachId}")]
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> DeleteCoachRating(string coachId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User Not Found");

            var coachRating = repository.GetQueryable()
                .Where(e => e.CoachId == coachId && e.TraineeId == user.Id)
                .FirstOrDefault();

            if (coachRating == null)
                return NotFound("Coach Rating not found");

            if (user.Id != coachRating.TraineeId)
                return Unauthorized("You don't have any access to delete this rating");

            repository.Delete(coachRating);
            if (await repository.SaveChangesAsync())
                return NoContent();
            return BadRequest("Error deleting rating");
        }
    }
}