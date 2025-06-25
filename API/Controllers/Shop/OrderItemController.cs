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
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemRepository _orderItemRepository;
        public OrderItemController(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetOrderItem(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new GeneralResponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _orderItemRepository.GetById(id,userId);
            if (result == null)
            {
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = "Failed to create OrderItem" });
            }
            return Ok(new GeneralResponse { IsSuccess = true, Data = result });
        }
        [HttpPost("AddToSpecificOrder")]
        [Authorize]
        public async Task<ActionResult> AddToSpecificOrder(AddOrderItemDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = ModelState.ExtractErrors() });
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new GeneralResponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _orderItemRepository.AddOrderItemInSpacificOrderWithCheckUserID(item, userId);
            if (result.Id == 0)
            {
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = "Failed to create OrderItem" });
            }
            return Ok(new GeneralResponse { IsSuccess = true, Data = "Created successfully" });
        }
        [HttpPost("AddToUnreceivedOrder")]
        [Authorize]
        public async Task<ActionResult> AddToUnreceivedOrder(AddOrderItemInOrderDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = ModelState.ExtractErrors() });
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new GeneralResponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _orderItemRepository.AddOrderItemInOrderDidnotReseived(item, userId);
            if (result.Id == 0)
            {
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = "Failed to create OrderItem" });
            }
            return Ok(new GeneralResponse { IsSuccess = true, Data = "Created successfully" });
        }
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> Update(EditOrderItemDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = ModelState.ExtractErrors() });
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new GeneralResponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _orderItemRepository.UpdateOrderItem(item, userId);
            if (result.Id == 0)
            {
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = "No OrderItem found with this Id" });
            }
            return Ok(new GeneralResponse { IsSuccess = true, Data = "Updated successfully" });
        }
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new GeneralResponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _orderItemRepository.DeleteOrderItemWithUserId(id, userId);
            if (result.Id == 0)
            {
                return BadRequest(new GeneralResponse { IsSuccess = false, Data = "No OrderItem found or you are not authorized" });
            }
            return Ok(new GeneralResponse { IsSuccess = true, Data = "Deleted successfully" });
        }
    }
}
