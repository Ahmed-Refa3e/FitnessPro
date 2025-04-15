using Core.DTOs.PostDTO;
using Core.Interfaces.Factories;
using Core.Interfaces.Repositories.PostRepositories;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> AddCoachPost([FromQuery] AddCoachPostDTO postDto)
        {
            if (ModelState.IsValid)
            {
                var repository = _factoryRepository.CreateRepository("COACH");
                var result = await repository.Add(postDto);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return Created("", _postRepository.GetPost(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpPost("AddGymPost")]
        public async Task<IActionResult> AddGymPost([FromQuery] AddGymPostDTO postDto)
        {
            if (ModelState.IsValid)
            {
                var repository = _factoryRepository.CreateRepository("GYM");
                var result = await repository.Add(postDto);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return Created("", _postRepository.GetPost(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpPost("AddShopPost")]
        public async Task<IActionResult> AddShopPost([FromQuery] AddShopPostDTO postDto)
        {
            if (ModelState.IsValid)
            {
                var repository = _factoryRepository.CreateRepository("SHOP");
                var result = await repository.Add(postDto);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return Created("", _postRepository.GetPost(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("DeletePost")]
        public IActionResult Delete([FromQuery] int id)
        {
            var result = _postRepository.DeletePost(id);
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
        public IActionResult AddLikeOnPost([FromQuery] AddLikeDTO addLikeDTO)
        {
            if (ModelState.IsValid)
            {
                var result = _postRepository.AddLikeOnPost(addLikeDTO);
                if (string.IsNullOrEmpty(result.Massage))
                {
                    return Created("", _postRepository.GetLike(result.Id));
                }
                return BadRequest(result.Massage);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("DeleteLikeFromPost")]
        public IActionResult DeleteLikeFromPost([FromQuery] string userId, [FromQuery] int postId)
        {
            var result = _postRepository.DeleteLikeFromPost(userId, postId);
            if (string.IsNullOrEmpty(result.Massage))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(result.Massage);
        }
        [HttpPost("AddCommentOnPost")]
        public IActionResult AddCommentOnPost([FromQuery] AddCommentDTO addCommentDTO)
        {
            if (ModelState.IsValid)
            {
                var result = _postRepository.AddComentOnPost(addCommentDTO);
                if (string.IsNullOrEmpty(result.Massage))
                {
                    return Created("", _postRepository.GetComment(result.Id));
                }
                return BadRequest(result.Massage);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("DeleteComment")]
        public IActionResult DeleteComment(int commentId)
        {
            var result = _postRepository.DeleteComment(commentId);
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
        public IActionResult AddLikeOnComment([FromQuery] AddLikeDTO addLikeDTO)
        {
            if (ModelState.IsValid)
            {
                var result = _postRepository.AddLikeOnComment(addLikeDTO);
                if (string.IsNullOrEmpty(result.Massage))
                {
                    return Created("", _postRepository.GetLike(result.Id));
                }
                return BadRequest(result.Massage);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("DeleteLikeFromComment")]
        public IActionResult DeleteLikeFromComment([FromQuery] string userId, [FromQuery] int commentId)
        {
            var result = _postRepository.DeleteLikeFromComment(userId, commentId);
            if (string.IsNullOrEmpty(result.Massage))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(result.Massage);
        }
        [HttpPost("AddCommentOnComment")]
        public IActionResult AddCommentOnComment([FromQuery] AddCommentDTO addCommentDTO)
        {
            if (ModelState.IsValid)
            {
                var result = _postRepository.AddComentOnComment(addCommentDTO);
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
