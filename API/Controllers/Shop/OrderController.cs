using Core.DTOs.ShopDTO;
using Core.Interfaces.Repositories.ShopRepositories;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult GetOrder(int id)
        {
            var result = _orderRepository.GetOrder(id);
            if (result is null)
            {
                return BadRequest("No Order has this id");
            }
            return Ok(result);
        }
        [HttpGet("UserOrders")]
        public ActionResult GetOrdersForUser(string userId)
        {
            var result = _orderRepository.GetOrdersForUser(userId);
            return Ok(result);
        }
        [HttpGet("ShopOrders")]
        public ActionResult GetOrdersForShop(int shopId)
        {
            var result = _orderRepository.GetOrdersForShop(shopId);
            return Ok(result);
        }
        [HttpPost]
        public ActionResult AddOrder(AddOrderDTO order)
        {
            if (ModelState.IsValid)
            {
                var result = _orderRepository.Add(order);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                var url = Url.Action(nameof(GetOrder), new { id = result.Id });
                return Created(url, _orderRepository.GetOrder(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpPut("makeItReseaved")]
        public ActionResult MakeOrderReseaved(int orderId)
        {
            var result = _orderRepository.MakeItReseved(orderId);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
        [HttpDelete]
        public ActionResult DeleteOrder(int orderId)
        {
            var result = _orderRepository.Delete(orderId);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
