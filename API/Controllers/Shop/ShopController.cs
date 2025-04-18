using Core.DTOs.ShopDTO;
using Core.Interfaces.Repositories.ShopRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers.Shop
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IShopRepository _repository;
        public ShopController(IShopRepository repository)
        {
            this._repository = repository;
        }
        [HttpGet]
        public ActionResult Get(int id)
        {
            var shop = _repository.GetShop(id);
            if (shop == null)
            {
                return BadRequest("No Shop has this Id");
            }
            return Ok(shop);
        }
        [HttpDelete]
        [Authorize(Roles = "Coach")]
        public ActionResult Delete(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _repository.Delete(id,userId);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent, "Deleted");
        }
        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Add(AddShopDTO shop)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _repository.Add(shop,userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                var url = Url.Action(nameof(Get), new { id = result.Id });
                return Created(url, _repository.GetShop(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpPut]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Update(UpdateShopDTO shop)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _repository.Update(shop, userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return StatusCode(StatusCodes.Status204NoContent, "Updated");
            }
            return BadRequest(ModelState);
        }
    }
}
