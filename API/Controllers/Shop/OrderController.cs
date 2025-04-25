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
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        [HttpGet("Order")]
        [Authorize]
        public async Task<ActionResult> GetOrder(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result =await _orderRepository.GetOrder(id, userId);
            if (result is null)
            {
                return NotFound(new Generalresponse { IsSuccess = false, Data = "No Order has this id" });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = result });
        }
        [HttpGet("UserOrders")]
        [Authorize]
        public async Task<ActionResult> GetOrdersForUser()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _orderRepository.GetOrdersForUser(userId);
            return Ok(new Generalresponse { IsSuccess = true, Data = result });
        }
        [HttpGet("ShopOrders")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> GetOrdersForShop(int shopId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _orderRepository.GetOrdersForShop(shopId,userId);
            return Ok(new Generalresponse { IsSuccess = true, Data = result });
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddOrder(AddOrderDTO order)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = ModelState.ExtractErrors() });
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _orderRepository.Add(order, userId);
            if (result.Id == 0)
            {
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = "Created successfully" });
        }
        [HttpPut("MarkOrderReceived")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> MarkOrderReceived(int orderId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _orderRepository.MakeItReseved(orderId, userId);
            if (result.Id == 0)
            {
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = "Updated successfully" });
        }
        [HttpPut("MakeOrderPaymented")]
        [Authorize]
        public async Task<ActionResult> MakeOrderPaymented(int orderId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _orderRepository.MakeItPaymented(orderId, userId);
            if (result.Id == 0)
            {
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = "Updated successfully" });
        }
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteOrder(int orderId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _orderRepository.Delete(orderId, userId);
            if (result.Id == 0)
            {
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = "Deleted successfully" });
        }
    }
}
