using Core.DTOs.ShopDTO;
using Core.Interfaces.Repositories.ShopRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers.Shop
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemRepository _orderItemRepository;
        public OrderItemController(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }
        [HttpGet]
        [Authorize]
        public ActionResult GetOrderItem(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _orderItemRepository.GetById(id,userId);
            if (result == null)
            {
                return BadRequest("No OrderItem has this Id");
            }
            return Ok(result);
        }
        [HttpPost("AddOrderItemInSpacificOrder")]
        [Authorize]
        public ActionResult AddOrderItemInSpacificOrder(AddOrderItemDTO item)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = _orderItemRepository.AddOrderItemInSpacificOrderWithCheckUserID(item,userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                var url = Url.Action(nameof(GetOrderItem), new { id = result.Id });
                return Created(url, _orderItemRepository.GetById(result.Id,userId));
            }
            return BadRequest(ModelState);
        }
        [HttpPost("AddOrderItemInOrderDidnotReseived")]
        [Authorize]
        public ActionResult AddOrderItemInOrderDidnotReseived(AddOrderItemInOrderDTO item)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = _orderItemRepository.AddOrderItemInOrderDidnotReseived(item, userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                var url = Url.Action(nameof(GetOrderItem), new { id = result.Id });
                return Created(url, _orderItemRepository.GetById(result.Id, userId));
            }
            return BadRequest(ModelState);
        }
        [HttpPut]
        [Authorize]
        public ActionResult Update(EditOrderItemDTO item)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = _orderItemRepository.UpdateOrderItem(item,userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _orderItemRepository.DeleteOrderItemWithUserId(id, userId);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
