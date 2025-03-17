using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Interfaces.Factories;
using Core.Interfaces.Repositories.PostRepositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Posts
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoachPostController : ControllerBase
    {
        protected readonly IPostRepositoryFactory _factoryRepository;
        protected readonly IPostRepresentationRepository _representationRepository;
        public CoachPostController(IPostRepositoryFactory factoryRepository, IPostRepresentationRepository representationRepository)
        {
            _factoryRepository = factoryRepository;
            _representationRepository = representationRepository;
        }
        [HttpPost]
        public async Task<IActionResult> AddPost([FromQuery] AddCoachPostDTO postDto)
        {
            if (ModelState.IsValid)
            {
                var repository = _factoryRepository.CreateRepository("COACH");
                var result = await repository.Add(postDto);
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
            var repository = _factoryRepository.CreateRepository("COACH");
            var result = repository.Delete(id);
            if (result.Id != 1)
            {
                return BadRequest(result.Massage);
            }
            return Ok("Post deleted successfully");
        }
    }
}
