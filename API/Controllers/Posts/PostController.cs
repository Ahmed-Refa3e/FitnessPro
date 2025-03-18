using Core.DTOs.PostDTO;
using Core.Interfaces.Factories;
using Core.Interfaces.Repositories.PostRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Posts
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        public PostController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        [HttpGet("{id:int}")]
        public IActionResult GetPost(int id)
        {
            var post = _postRepository.Get(id);
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
            var result=_postRepository.DeleteLikeFromPost(userId, postId);
            if (string.IsNullOrEmpty(result.Massage))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(result.Massage);
        }
        [HttpPost("AddCommentOnPost")]
        public IActionResult AddCommentOnPost([FromQuery]AddCommentDTO addCommentDTO)
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
        [HttpDelete("DeleteCommentFromPost")]
        public IActionResult DeleteCommentFromPost(int commentId)
        {
            var result = _postRepository.DeleteCommentFromPost(commentId);
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
    }
}
