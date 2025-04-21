using Core.DTOs.ShopDTO;
using Core.DTOs.ShopDTO.ProductDTO;
using Core.Interfaces.Repositories.ShopRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult GetById(int id)
        {
            var result = _productRepository.GetProductById(id);
            if (result is null)
            {
                return BadRequest("No product has this Id.");
            }
            return Ok(result);
        }
        [HttpGet("Categories")]
        public ActionResult GetAllProductCategories()
        {
            var result = _categoryRepository.GetAll();
            return Ok(result);
        }
        [HttpGet("ProductsNoCategory")]
        public ActionResult GetProductsNoCategory(int page, int size)
        {
            var result = _productRepository.GetProductsInPaginationsNoCategory(page, size);
            return Ok(result);
        }
        [HttpGet("ProductsInCategory")]
        public ActionResult GetProductsInCategory(int page, int size, int category)
        {
            var result = _productRepository.GetProductsInPaginationsWithCategory(page, size, category);
            return Ok(result);
        }
        [HttpGet("ShopProductsNoCategory")]
        public ActionResult GetShopProductsNoCategory(int shopId, int page, int size)
        {
            var result = _productRepository.GetProductsInPaginationsOnShopNoCategory(shopId, page, size);
            return Ok(result);
        }
        [HttpGet("ShopProductsInCategory")]
        public ActionResult GetShopProductsInCategory(int shopId,int page, int size, int category)
        {
            var result = _productRepository.GetProductsInPaginationsOnShopWithCategory(shopId, page, size, category);
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Add(AddProductDTO product)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _productRepository.Add(product,userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                var url = Url.Action(nameof(GetById), new { id = result.Id });
                return Created(url, _productRepository.GetProductById(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpDelete]
        [Authorize(Roles = "Coach")]
        public ActionResult Delete(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _productRepository.Delete(id,userId);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
        [HttpPut("Details")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult> Update(EditProductDTO product, int id)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _productRepository.Update(product, id,userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(ModelState);
        }
        [HttpPut("UpdateCategoriesIOfProduct")]
        [Authorize(Roles = "Coach")]
        public ActionResult UpdateCategoriesInProduct(ModifyCategoriesInProductDTO categories)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = _productRepository.UpdateCategoriesOfProduct(categories, userId);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(ModelState);
        }
    }
}
