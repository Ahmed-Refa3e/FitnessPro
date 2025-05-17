using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Interfaces.Factories;
using Core.Interfaces.Repositories.PostRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;
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
        public async Task<IActionResult> AddCoachPost(AddPostDTO dto) => await AddPost(dto, "COACH");

        [HttpPost("AddGymPost")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> AddGymPost(AddGymPostDTO dto) => await AddPost(dto, "GYM");

        [HttpPost("AddShopPost")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> AddShopPost(AddShopPostDTO dto) => await AddPost(dto, "SHOP");

        private async Task<IActionResult> AddPost(AddPostDTO postDto, string type)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = ModelState.ExtractErrors() });

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });

            var repository = _factoryRepository.CreateRepository(type);
            var result = await repository.Add(postDto, userId);

            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Created successfully" });
        }
        [HttpGet("PostsForUser")]
        public async Task<IActionResult> PostsForUser(int pageNumber)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value??"";
            var posts = await _postRepository.GetPostsForUserFromFollowers(pageNumber,userId);
            if (posts == null)
            {
                return NotFound(new Generalresponse { IsSuccess = false, Data = "No Post found with this ID." });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = posts });
        }
        [HttpGet("GetAllPostsOfShop/{id:int}/{pageNumber:int}")]
        public async Task<IActionResult> PostsOfShop(int id, int pageNumber)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var posts = await _postRepository.GetPostsOfShop(id,pageNumber,userId);
            if (posts == null)
            {
                return NotFound(new Generalresponse { IsSuccess = false, Data = "No Post found with this ID." });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = posts });
        }
        [HttpGet("GetAllPostsOfGym/{id:int}/{pageNumber:int}")]
        public async Task<IActionResult> PostsOfGym(int id, int pageNumber)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var posts = await _postRepository.GetPostsOfGym(id, pageNumber, userId);
            if (posts == null)
            {
                return NotFound(new Generalresponse { IsSuccess = false, Data = "No Post found with this ID." });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = posts });
        }
        [HttpGet("GetAllPostsOfCoach/{id}/{pageNumber:int}")]
        public async Task<IActionResult> PostsOfShop(string id,int pageNumber)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var posts = await _postRepository.GetPostsOfCoach(id, pageNumber, userId);
            if (posts == null)
            {
                return NotFound(new Generalresponse { IsSuccess = false, Data = "No Post found with this ID." });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = posts });
        }
        [HttpDelete("DeletePost/{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _postRepository.DeletePost(id, userId);
            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Deleted successfully" });
        }
        [HttpGet("GetPost/{id:int}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _postRepository.GetPost(id);
            if (post == null)
            {
                return NotFound(new Generalresponse { IsSuccess = false, Data = "No Post found with this ID." });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = post });
        }
        [HttpPost("AddLikeOnPost")]
        [Authorize]
        public async Task<IActionResult> AddLikeOnPost(AddLikeOnPostDTO addLikeDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = ModelState.ExtractErrors() });
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _postRepository.AddLikeOnPost(addLikeDTO, userId);
            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Created successfully" });
        }
        [HttpDelete("DeleteLikeFromPost/{postId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteLikeFromPost(int postId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _postRepository.DeleteLikeFromPost(userId, postId);
            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Deleted successfully" });
        }
        [HttpPost("AddCommentOnPost")]
        [Authorize]
        public async Task<IActionResult> AddCommentOnPost(AddCommentOnPostDTO addCommentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = ModelState.ExtractErrors() });
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _postRepository.AddCommentOnPost(addCommentDTO, userId);
            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Added successfully" });
        }
        [HttpDelete("DeleteComment/{commentId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _postRepository.DeleteComment(commentId, userId);
            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Deleted successfully" });
        }
        [HttpGet("GetPostLikes/{id:int}")]
        public async Task<IActionResult> GetPostLikes(int id)
        {
            var result = await _postRepository.GetLikeListOnPost(id);
            if (result is null)
            {
                return NotFound(new Generalresponse { IsSuccess = false, Data = "No post has this id." });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = result });
        }
        [HttpPost("AddLikeOnComment")]
        [Authorize]
        public async Task<IActionResult> AddLikeOnComment(AddLikeOnCommentDTO addLikeDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = ModelState.ExtractErrors() });
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _postRepository.AddLikeOnComment(addLikeDTO, userId);
            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Added successfully" });
        }
        [HttpDelete("DeleteLikeFromComment/{commentId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteLikeFromComment(int commentId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _postRepository.DeleteLikeFromComment(userId, commentId);
            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Deleted successfully" });
        }
        [HttpPost("AddCommentOnComment")]
        [Authorize]
        public async Task<IActionResult> AddCommentOnComment(AddCommentOnCommentDTO addCommentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = ModelState.ExtractErrors() });
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _postRepository.AddCommentOnComment(addCommentDTO, userId);
            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Added successfully" });
        }
        [HttpGet("GetCommentLikes/{id:int}")]
        public async Task<IActionResult> GetCommentLikes(int id)
        {
            var result = await _postRepository.GetLikeListOnComment(id);
            if (result is null)
            {
                return NotFound(new Generalresponse { IsSuccess = false, Data = "No comment found with this ID." });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = result });
        }
        [HttpGet("GetComment{id:int}")]
        public async Task<IActionResult> GetComment(int id)
        {
            var result = await _postRepository.GetComment(id);
            if (result is null)
            {
                return NotFound(new Generalresponse { IsSuccess = false, Data = "No comment found with this ID." });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = result });
        }
    }
}
