using Core.DTOs.PostDTO;
using Core.Interfaces.Factories;
using Core.Interfaces.Repositories.PostRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers.Posts
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostRepositoryFactory _factoryRepository;
        public PostController(IPostRepositoryFactory factoryRepository, IPostRepository postRepository)
        {
            _postRepository = postRepository;
            _factoryRepository = factoryRepository;
        }
        [HttpPost("AddCoachPost")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> AddCoachPost(AddPostDTO postDto)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var repository = _factoryRepository.CreateRepository("COACH");
                var result = await repository.Add(postDto, userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return Created("", _postRepository.GetPost(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpPost("AddGymPost")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> AddGymPost(AddGymPostDTO postDto)
        {
            if (ModelState.IsValid)
            {
                var userId= User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var repository = _factoryRepository.CreateRepository("GYM");
                var result = await repository.Add(postDto,userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return Created("", _postRepository.GetPost(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpPost("AddShopPost")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> AddShopPost(AddShopPostDTO postDto)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var repository = _factoryRepository.CreateRepository("SHOP");
                var result = await repository.Add(postDto, userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return Created("", _postRepository.GetPost(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("DeletePost")]
        [Authorize(Roles = "Coach")]
        public IActionResult Delete(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value??"";
            var result = _postRepository.DeletePost(id, userId);
            if (string.IsNullOrEmpty(result.Massage))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(result.Massage);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetPost(int id)
        {
            var post = _postRepository.GetPost(id);
            if (post == null)
            {
                return NotFound("Post not found");
            }
            return Ok(post);
        }
        [HttpPost("AddLikeOnPost")]
        [Authorize]
        public IActionResult AddLikeOnPost(AddLikeDTO addLikeDTO)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = _postRepository.AddLikeOnPost(addLikeDTO, userId);
                if (string.IsNullOrEmpty(result.Massage))
                {
                    return Created("", _postRepository.GetLike(result.Id));
                }
                return BadRequest(result.Massage);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("DeleteLikeFromPost")]
        [Authorize]
        public IActionResult DeleteLikeFromPost(int postId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = _postRepository.DeleteLikeFromPost(userId, postId);
            if (string.IsNullOrEmpty(result.Massage))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(result.Massage);
        }
        [HttpPost("AddCommentOnPost")]
        [Authorize]
        public IActionResult AddCommentOnPost(AddCommentDTO addCommentDTO)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = _postRepository.AddCommentOnPost(addCommentDTO, userId);
                if (string.IsNullOrEmpty(result.Massage))
                {
                    return Created("", _postRepository.GetComment(result.Id));
                }
                return BadRequest(result.Massage);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("DeleteComment")]
        [Authorize]
        public IActionResult DeleteComment(int commentId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = _postRepository.DeleteComment(commentId,userId);
            if (string.IsNullOrEmpty(result.Massage))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(result.Massage);
        }
        [HttpGet("GetLikesOnPost")]
        public IActionResult GetLikeListOnPost(int id)
        {
            var result = _postRepository.GetLikeListOnPost(id);
            if (result is null)
            {
                return BadRequest("No Post has this Id");
            }
            return Ok(result);
        }
        [HttpPost("AddLikeOnComment")]
        [Authorize]
        public IActionResult AddLikeOnComment(AddLikeDTO addLikeDTO)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = _postRepository.AddLikeOnComment(addLikeDTO,userId);
                if (string.IsNullOrEmpty(result.Massage))
                {
                    return Created("", _postRepository.GetLike(result.Id));
                }
                return BadRequest(result.Massage);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("DeleteLikeFromComment")]
        [Authorize]
        public IActionResult DeleteLikeFromComment(int commentId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = _postRepository.DeleteLikeFromComment(userId, commentId);
            if (string.IsNullOrEmpty(result.Massage))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(result.Massage);
        }
        [HttpPost("AddCommentOnComment")]
        [Authorize]
        public IActionResult AddCommentOnComment(AddCommentDTO addCommentDTO)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = _postRepository.AddCommentOnComment(addCommentDTO,userId);
                if (string.IsNullOrEmpty(result.Massage))
                {
                    return Created("", _postRepository.GetComment(result.Id));
                }
                return BadRequest(result.Massage);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetLikesOnComment")]
        public IActionResult GetLikeListOnComment(int id)
        {
            var result = _postRepository.GetLikeListOnComment(id);
            if (result is null)
            {
                return BadRequest("No Post has this Id");
            }
            return Ok(result);
        }
        [HttpGet("GetComment")]
        public IActionResult GetComment(int id)
        {
            var result = _postRepository.GetComment(id);
            if (result is null)
            {
                return BadRequest("No Post has this Id");
            }
            return Ok(result);
        }
    }
}
