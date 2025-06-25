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
            _repository = repository;
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var shop = await _repository.GetShop(id);
            if (shop == null)
            {
                return NotFound(new GeneralResponse { IsSuccess = false, Data = "No shop found with this ID." });
            }
            return Ok(new GeneralResponse { IsSuccess = true, Data = shop });
        }
        [HttpGet("ShopsOfOwner")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> GetCoachShops()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new GeneralResponse { IsSuccess = false, Data = "User not logged in." });
            var shop = await _repository.GetShopsOfOwner(userId);
            if (shop == null)
            {
                return NotFound(new GeneralResponse { IsSuccess = false, Data = "No shop found with this ID." });
            }
            return Ok(new GeneralResponse { IsSuccess = true, Data = shop });
        }
        [HttpGet("ShopSearch")]
        public async Task<ActionResult> GetShops([FromQuery] SearchShopDTO searchDTO)
        {
            var shop = await _repository.GetShopsWithFilter(searchDTO);
            return Ok(new GeneralResponse { IsSuccess = true, Data = shop });
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new GeneralResponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _repository.Delete(userId, id);
            if (result.Id == 0)
            {
                return NotFound(new GeneralResponse { IsSuccess = false, Data = result.Massage });
            }
            return Ok(new GeneralResponse { IsSuccess = true, Data = "Deleted" });
        }
        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Add(AddShopDTO shop)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = ModelState.ExtractErrors() });

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new GeneralResponse { IsSuccess = false, Data = "User not logged in." });

            var result = await _repository.Add(shop, userId);
            if (result.Id == 0)
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = result.Massage });

            return Ok(new GeneralResponse { IsSuccess = true, Data = "Created successfully" });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Update([FromBody] UpdateShopDTO shop, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = ModelState.ExtractErrors() });

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new GeneralResponse { IsSuccess = false, Data = "User not logged in." });

            var result = await _repository.Update(shop, id, userId);
            if (result.Id == 0)
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = result.Massage });

            return Ok(new GeneralResponse { IsSuccess = true, Data = "Updated successfully" });
        }
        [HttpPut("UpdateImage/{id:int}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> UpdateImage(UpdateImageDTO imageDTO, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = ModelState.ExtractErrors() });

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new GeneralResponse { IsSuccess = false, Data = "User not logged in." });

            var result = await _repository.UpdateImage(imageDTO, id, userId);
            if (result.Id == 0)
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = result.Massage });

            return Ok(new GeneralResponse { IsSuccess = true, Data = "Updated successfully" });
        }
    }
}
