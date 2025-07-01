using Core.DTOs.GeneralDTO;
using Core.DTOs.GymDTO;
using Core.Entities.GymEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;
using System.Security.Claims;

namespace API.Controllers.GymAndRating
{
    public class GymRatingController(IUnitOfWork unitOfWork) : BaseApiController
    {
        private readonly IGenericRepository<GymRating> _ratingRepo = unitOfWork.Repository<GymRating>();

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GymRatingResponseDTO>> GetGymRatingById(int id)
        {
            var gymRating = await _ratingRepo.GetByIdAsync(id);
            if (gymRating == null) return NotFound("GymRating not found");

            return Ok(gymRating.ToResponseDto());
        }

        [HttpPost]
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> CreateGymRating(CreateGymRatingDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var traineeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingRating = await _ratingRepo
                .GetQueryable()
                .FirstOrDefaultAsync(gr => gr.TraineeID == traineeId && gr.GymID == dto.GymID);

            if (existingRating is not null)
                return BadRequest("You have already rated this gym");

            var rating = dto.ToEntity();
            rating.TraineeID = traineeId;

            _ratingRepo.Add(rating);

            if (await unitOfWork.CompleteAsync())
                return CreatedAtAction(nameof(GetGymRatingById), new { id = rating.GymRatingID }, rating.ToResponseDto());

            return BadRequest("Problem adding gym rating");
        }

        [HttpPut("{gymId:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult> UpdateGymRating(int gymId, UpdateGymRatingDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var traineeId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var gymRating = await _ratingRepo
                .GetQueryable()
                .FirstOrDefaultAsync(gr => gr.TraineeID == traineeId && gr.GymID == gymId);

            if (gymRating is null)
                return NotFound($"No rating found for GymID {gymId} by this user.");

            gymRating.RatingDate = DateTime.Now;
            gymRating.RatingValue = dto.RatingValue;
            gymRating.Review = dto.Review;

            if (!await unitOfWork.CompleteAsync())
                return StatusCode(500, "Failed to update the gym rating.");

            return Ok(new GeneralResponse { IsSuccess = true, Data = "Gym rating updated successfully" });
        }

        [HttpDelete("{gymId:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult> DeleteGymRating(int gymId)
        {
            var traineeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var gymRating = await _ratingRepo
                .GetQueryable()
                .FirstOrDefaultAsync(gr => gr.TraineeID == traineeId && gr.GymID == gymId);

            if (gymRating is null)
                return NotFound($"No rating found for GymID {gymId} by this user.");

            _ratingRepo.Remove(gymRating);

            if (!await unitOfWork.CompleteAsync())
                return StatusCode(500, "Failed to delete the gym rating.");

            return Ok(new GeneralResponse { IsSuccess = true, Data = "Gym rating deleted successfully" });
        }

        [HttpGet("hasRated/{gymId:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult<bool>> HasRatedGym(int gymId)
        {
            var traineeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var hasRated = await _ratingRepo
                .GetQueryable()
                .AnyAsync(gr => gr.TraineeID == traineeId && gr.GymID == gymId);

            return Ok(hasRated);
        }

        [HttpGet("UserRating/{gymId:int}")]
        [Authorize(Roles = "Trainee")]
        public async Task<ActionResult<GymRatingResponseDTO>> GetUserGymRating(int gymId)
        {
            var traineeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userRating = await _ratingRepo
                .GetQueryable()
                .FirstOrDefaultAsync(gr => gr.TraineeID == traineeId && gr.GymID == gymId);

            if (userRating is null)
                return NotFound("User has not rated this gym");

            return Ok(userRating.ToResponseDto());
        }
    }
}
