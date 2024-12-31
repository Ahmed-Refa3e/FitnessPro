using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Interfaces.Repositories.PostRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Posts
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopPostController : PostControllerBase<AddShopPostDTO, ShopPost>
    {
        public ShopPostController(IPostRepository repository, IPostRepresentationRepository representationRepository) : base(repository, representationRepository)
        {
        }
    }
}
