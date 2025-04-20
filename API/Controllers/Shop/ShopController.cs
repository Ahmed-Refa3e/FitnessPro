using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.Interfaces.Repositories.ShopRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;
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
        public async Task<ActionResult> Get(int id)
        {
            var shop = await _repository.GetShop(id);
            if (shop == null)
            {
                return NotFound("No shop found with this ID.");
            }
            return Ok(shop);
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _repository.DeleteAsync(id, userId);
            if (result.Id == 0)
            {
                return NotFound(result.Massage);
            }
            return NoContent();
        }
        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Add([FromBody] AddShopDTO shop)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = ModelState.ExtractErrors() });
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in.");
            var result = await _repository.Add(shop, userId);
            if (result.Id == 0)
                return BadRequest(result.Massage);
            return Created();
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Update([FromBody] AddShopDTO shop, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = ModelState.ExtractErrors() });
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in.");
            var result = await _repository.Update(shop, id, userId);
            if (result.Id == 0)
                return BadRequest(result.Massage);
            return NoContent();
        }
    }
}
