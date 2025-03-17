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
        private readonly IPostRepresentationRepository _representationRepository;
        public PostController(IPostRepresentationRepository representationRepository)
        {
            _representationRepository = representationRepository;
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
    }
}
