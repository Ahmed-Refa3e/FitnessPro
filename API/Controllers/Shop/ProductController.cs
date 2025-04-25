using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.DTOs.ShopDTO.ProductDTO;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.ShopRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Extensions;
using System.Security.Claims;

namespace API.Controllers.Shop
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            var result = await _productRepository.GetProductById(id);
            if (result is null)
            {
                return NotFound(new Generalresponse { IsSuccess = false, Data = "No product has this Id." });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = result });
        }
        [HttpGet("ProductCategories")]
        public async Task<ActionResult> GetAllProductCategories()
        {
            var result =await _categoryRepository.GetAll();
            return Ok(new Generalresponse { IsSuccess = true, Data = result });
        }
        [HttpGet("Products")]
        public async Task<ActionResult> GetProducts([FromQuery]ProductSearchDTO searchDTO)
        {
            var result = await _productRepository.GetProducts(searchDTO);
            return Ok(new Generalresponse { IsSuccess = true, Data = result });
        }
        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Add(AddProductDTO product)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = ModelState.ExtractErrors() });
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _productRepository.Add(product, userId);
            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Created successfully" });
        }
        [HttpDelete]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _productRepository.Delete(id,userId);
            if (result.Id == 0)
            {
                return NotFound(new Generalresponse { IsSuccess = false, Data = result.Massage });
            }
            return Ok(new Generalresponse { IsSuccess = true, Data = "Deleted" });
        }
        [HttpPut("Details")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Update(EditProductDTO product, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = ModelState.ExtractErrors() });

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result = await _productRepository.Update(product, id, userId);
            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Updated successfully" });
        }
        [HttpPut("UpdateCategoriesOfProduct")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> UpdateCategoriesInProduct(ModifyCategoriesInProductDTO categories)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = ModelState.ExtractErrors() });

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse { IsSuccess = false, Data = "User not logged in." });
            var result =await _productRepository.UpdateCategoriesOfProduct(categories, userId);
            if (result.Id == 0)
                return BadRequest(new Generalresponse { IsSuccess = false, Data = result.Massage });

            return Ok(new Generalresponse { IsSuccess = true, Data = "Updated successfully" });
        }
    }
}
