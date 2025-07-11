﻿using Core.DTOs.UserDTO;
using Core.Entities.OnlineTrainingEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoachRatingController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork) : BaseApiController
    {
        private readonly IGenericRepository<CoachRating> _ratingRepo = unitOfWork.Repository<CoachRating>();

        [HttpPost]
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> CreateCoachRating(CreateCoachRatingDTO coachRatingDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User Not Found");

            var ratingFromDb = await _ratingRepo.GetQueryable()
                .FirstOrDefaultAsync(e => e.TraineeId == user.Id && e.CoachId == coachRatingDTO.coachId);
            if (ratingFromDb is not null)
                return Conflict($"You already add rating to this Coach");

            var coachRating = new CoachRating()
            {
                TraineeId = user.Id,
                CoachId = coachRatingDTO.coachId,
                Content = coachRatingDTO.Review,
                CreatedAt = DateTime.UtcNow,
                Rating = coachRatingDTO.ratingValue
            };
            _ratingRepo.Add(coachRating);

            var saved = await unitOfWork.CompleteAsync();
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
            var coachRating = await _ratingRepo.GetByIdAsync(id);
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


            var coachRating = _ratingRepo.GetQueryable()
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

            var query = _ratingRepo.GetQueryable().Where(e => e.CoachId == CoachId);

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

        [Authorize]
        [HttpGet("hasRated/{coachId}")]
        public async Task<IActionResult> CheckCoachRating(string coachId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User Not Found");

            var hasRated = _ratingRepo.GetQueryable()
                 .Any(r => r.CoachId == coachId && r.TraineeId == user.Id);

            return Ok(hasRated);
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


            var coachRating = _ratingRepo.GetQueryable()
                .Where(e => e.CoachId == coachId && e.TraineeId == user.Id)
                .FirstOrDefault();

            if (coachRating == null)
                return NotFound("You didn't add any rating");

            if (user?.Id != coachRating.TraineeId)
                return Unauthorized("You can't update this rating");

            coachRating.Rating = coachRatingDTO.RatingValue;
            coachRating.Content = coachRatingDTO.Content;
            coachRating.CreatedAt = DateTime.Now;

            _ratingRepo.Update(coachRating);
            if (!await unitOfWork.CompleteAsync())
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

            var coachRating = _ratingRepo.GetQueryable()
                .Where(e => e.CoachId == coachId && e.TraineeId == user.Id)
                .FirstOrDefault();

            if (coachRating == null)
                return NotFound("Coach Rating not found");

            if (user.Id != coachRating.TraineeId)
                return Unauthorized("You don't have any access to delete this rating");

            _ratingRepo.Remove(coachRating);
            if (await unitOfWork.CompleteAsync())
                return NoContent();
            return BadRequest("Error deleting rating");
        }
    }
}