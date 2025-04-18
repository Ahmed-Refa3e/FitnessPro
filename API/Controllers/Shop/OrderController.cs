using Core.DTOs.ShopDTO;
using Core.Interfaces.Repositories.ShopRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers.Shop
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        [HttpGet("Order")]
        [Authorize]
        public ActionResult GetOrder(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _orderRepository.GetOrder(id, userId);
            if (result is null)
            {
                return BadRequest("No Order has this id");
            }
            return Ok(result);
        }
        [HttpGet("UserOrders")]
        [Authorize]
        public ActionResult GetOrdersForUser()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _orderRepository.GetOrdersForUser(userId);
            return Ok(result);
        }
        [HttpGet("ShopOrders")]
        [Authorize(Roles = "Coach")]
        public ActionResult GetOrdersForShop(int shopId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _orderRepository.GetOrdersForShop(shopId,userId);
            return Ok(result);
        }
        [HttpPost]
        [Authorize]
        public ActionResult AddOrder(AddOrderDTO order)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = _orderRepository.Add(order,userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                var url = Url.Action(nameof(GetOrder), new { id = result.Id, userId = userId });
                return Created(url, _orderRepository.GetOrder(result.Id,userId));
            }
            return BadRequest(ModelState);
        }
        [HttpPut("makeItReseaved")]
        [Authorize(Roles = "Coach")]
        public ActionResult MakeOrderReseaved(int orderId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _orderRepository.MakeItReseved(orderId,userId);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
        [HttpPut("makeItPaymented")]
        [Authorize]
        public ActionResult makeItPaymented(int orderId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _orderRepository.MakeItPaymented(orderId, userId);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
        [HttpDelete]
        [Authorize]
        public ActionResult DeleteOrder(int orderId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _orderRepository.Delete(orderId, userId);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
