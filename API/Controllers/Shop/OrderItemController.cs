using Core.DTOs.ShopDTO;
using Core.Interfaces.Repositories.ShopRepositories;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult GetOrderItem(int id)
        {
            var result = _orderItemRepository.GetById(id);
            if (result == null)
            {
                return BadRequest("No OrderItem has this Id");
            }
            return Ok(result);
        }
        [HttpPost("AddOrderItemInSpacificOrder")]
        public ActionResult AddOrderItemInSpacificOrder(AddOrderItemDTO item)
        {
            if (ModelState.IsValid)
            {
                var result = _orderItemRepository.AddOrderItemInSpacificOrder(item);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                var url = Url.Action(nameof(GetOrderItem), new { id = result.Id });
                return Created(url, _orderItemRepository.GetById(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpPost("AddOrderItemInOrderDidnotReseived")]
        public ActionResult AddOrderItemInOrderDidnotReseived(AddOrderItemInOrderDTO item, string userId)
        {
            if (ModelState.IsValid)
            {
                var result = _orderItemRepository.AddOrderItemInOrderDidnotReseived(item, userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                var url = Url.Action(nameof(GetOrderItem), new { id = result.Id });
                return Created(url, _orderItemRepository.GetById(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpPut]
        public ActionResult Update(EditOrderItemDTO item)
        {
            if (ModelState.IsValid)
            {
                var result = _orderItemRepository.UpdateOrderItem(item);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var result = _orderItemRepository.DeleteOrderItem(id);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
