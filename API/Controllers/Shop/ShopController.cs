using Core.DTOs.ShopDTO;
using Core.Interfaces.Repositories.ShopRepositories;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("{id:int}")]
        public ActionResult Get(int id)
        {
            var shop = _repository.GetShop(id);
            if (shop == null)
            {
                return BadRequest("No Shop has this Id");
            }
            return Ok(shop);
        }
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var result = _repository.Delete(id);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent, "Deleted");
        }
        [HttpPost]
        public async Task<ActionResult> Add(AddShopDTO shop)
        {
            if (ModelState.IsValid)
            {
                var result = await _repository.Add(shop);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                var url = Url.Action(nameof(Get), new { id = result.Id });
                return Created(url, _repository.GetShop(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(UpdateShopDTO shop, int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _repository.Update(shop, id);
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
