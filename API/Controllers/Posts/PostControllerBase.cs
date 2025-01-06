using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Interfaces.Repositories.PostRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Posts
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostControllerBase<TPostDto, TPost> : ControllerBase
        where TPostDto : AddPostDTO
        where TPost : Post
    {
        protected readonly IPostRepository _repository;
        protected readonly IPostRepresentationRepository _representationRepository;
        public PostControllerBase(IPostRepository repository, IPostRepresentationRepository representationRepository)
        {
            _repository = repository;
            _representationRepository = representationRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddPost([FromForm] TPostDto postDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _repository.Add(postDto);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                string url = Url.Action(nameof(GetPost), new { id = result.Id });
                return Created(url, _representationRepository.Get(result.Id));
            }
            return BadRequest(ModelState);
        }

        [HttpGet("{id}")]
        public IActionResult GetPost(int id)
        {
            var post = _representationRepository.Get(id);
            if (post == null)
            {
                return NotFound("Post not found");
            }
            return Ok(post);
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePost(int id)
        {
            var result = _repository.Delete(id);
            if (result.Id != 1)
            {
                return BadRequest(result.Massage);
            }
            return Ok("Post deleted successfully");
        }
    }
}

